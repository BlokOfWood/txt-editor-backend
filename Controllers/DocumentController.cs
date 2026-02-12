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
    [ValidateUserId]
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
    [ValidateUserId]
    public async Task<IActionResult> AddNewDocument([FromBody] CreateDocumentDto document)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        bool success = await _documentService.CreateNewDocument(document, userId);

        return success ? Ok() : Conflict();
    }

    [HttpPost("{id}")]
    [Authorize]
    [ValidateUserId]
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
    [ValidateUserId]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        var result = await _documentService.GetDocumentById(id, userId);

        return result is not null ? Ok(result) : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ValidateUserId]
    public async Task<IActionResult> DeleteDocumentById(int id)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        var result = await _documentService.DeleteDocument(id, userId);

        return result ? Ok() : NotFound();
    }

    [Route("ws")]
    [Authorize]
    [ValidateUserId]
    public async Task DocumentStateWebsocket()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        await _occupancyService.NewSessionAsync(webSocket);
    }

#if MOCKING

    [HttpPost("mock")]
    [Authorize]
    [ValidateUserId]
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