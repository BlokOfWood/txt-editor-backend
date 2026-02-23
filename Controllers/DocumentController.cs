using aresu_txt_editor_backend.Filters;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aresu_txt_editor_backend.Controllers;

[ApiController]
[Route("document")]
public class DocumentController(IDocumentService _documentService, IOccupancyService _occupancyService, ILogger<DocumentController> _logger) : ControllerBase
{
    public const uint MaxRequestableDocumentCount = 100;

    [HttpGet]
    [Authorize]
    [GetUserId]
    public async Task<IActionResult> GetDocumentTitles([FromQuery] string? query, [FromQuery] int offset, [FromQuery] int quantity)
    {
        if (MaxRequestableDocumentCount < quantity)
        {
            return BadRequest($"Maximum {MaxRequestableDocumentCount} pages may be requested at any time.");
        }

        int userId = (int)HttpContext.Items["UserId"]!;

        var documents = _documentService.GetUserDocuments(query ?? "", offset, quantity, userId);

        return Ok(await documents);
    }

    [HttpPost]
    [Authorize]
    [GetUserId]
    public async Task<IActionResult> AddNewDocument([FromBody] CreateDocumentDto document)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        bool success = await _documentService.CreateNewDocument(document, userId);

        return success ? Ok() : Conflict();
    }

    [HttpPost("{id}")]
    [Authorize]
    [GetUserId]
    [ServiceFilter(typeof(ValidateSessionIdAttribute))]
    public async Task<IActionResult> ModifyDocumentText(int id, [FromBody] ModifyDocumentDto document)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        if (document.Title == null && document.Content == null)
            return BadRequest();

        var result = await _documentService.UpdateDocument(id, userId, document);

        return result ? Ok() : NotFound();
    }

    [HttpGet("{id}")]
    [Authorize]
    [GetUserId]
    [ServiceFilter(typeof(ValidateSessionIdAttribute))]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        var result = await _documentService.GetDocumentById(id, userId);

        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost("{id}/occupy")]
    [Authorize]
    [GetUserId]
    [GetSessionId]
    public async Task<IActionResult> OccupyDocument(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;
        long sessionId = (long)HttpContext.Items["SessionId"]!;

        var tryOccupyDocumentResult = _occupancyService.TryOccupyDocument(userId, sessionId, id);

        return tryOccupyDocumentResult switch
        {
            DocumentLockOpResult.INVALID_SESSION_ID or DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH => Unauthorized(),
            DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION => Conflict(),
            DocumentLockOpResult.SUCCESS => Ok(),
            _ => throw new Exception($"Unexpected result {tryOccupyDocumentResult} returned from TryOccupyDocument()."),
        };
    }

    [HttpPost("{id}/unoccupy")]
    [Authorize]
    [GetUserId]
    [ServiceFilter(typeof(ValidateSessionIdAttribute))]
    public async Task<IActionResult> UnoccupyDocument(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;
        long sessionId = (long)HttpContext.Items["SessionId"]!;

        var tryUnoccupyDocumentResult = _occupancyService.TryRemoveDocumentLock(userId, sessionId, id);

        return tryUnoccupyDocumentResult switch
        {
            DocumentLockOpResult.DOCUMENT_UNOCCUPIED => BadRequest(),
            DocumentLockOpResult.SUCCESS => Ok(),
            _ => throw new Exception($"Unexpected result {tryUnoccupyDocumentResult} returned from TryRemoveDocumentLock()."),
        };
    }

    [HttpDelete("{id}")]
    [Authorize]
    [GetUserId]
    [ServiceFilter(typeof(ValidateSessionIdAttribute))]
    public async Task<IActionResult> DeleteDocumentById(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        var isDocumentOccupiedResult = (DocumentLockOpResult)HttpContext.Items["SessionState"]!;

        if (isDocumentOccupiedResult == DocumentLockOpResult.DOCUMENT_OCCUPIED_BY_SESSION)
            return Conflict();

        var result = await _documentService.DeleteDocument(id, userId);

        return result ? Ok() : NotFound();
    }

    [Route("ws")]
    [Authorize]
    [GetUserId]
    public async Task DocumentStateWebsocket()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        int userId = (int)HttpContext.Items["UserId"]!;
        await _occupancyService.NewSessionAsync(userId, webSocket);
    }

#if MOCKING

    [HttpPost("mock")]
    [Authorize]
    [GetUserId]
    public async Task<IActionResult> CreateMockDocuments([FromQuery] int quantity)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        _logger.LogDebug($"Creating {quantity} mock documents.");

        if (quantity > 50000)
            return BadRequest();

        await _documentService.AddTestDocuments(quantity, userId);

        return Ok();
    }
#endif
}