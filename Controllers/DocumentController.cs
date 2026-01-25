using aresu_txt_editor_backend.Filters;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aresu_txt_editor_backend.Controllers;

[ApiController]
[Route("document")]
public class DocumentController(IDocumentService _documentService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDocumentTitles()
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        // TODO: implement paging 
        var documents = _documentService.GetUserDocuments(userId);

        return Ok(await documents);
    }

    [HttpPost()]
    [Authorize]
    public async Task<IActionResult> AddNewDocument([FromBody] CreateDocumentDto document)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        await _documentService.CreateNewDocument(document, userId);

        return Ok();
    }

    [HttpPost("{id}")]
    [Authorize]
    [ValidateUserId]
    public async Task<IActionResult> ModifyDocumentText(int id, [FromBody] ModifyDocumentDto document)
    {
        int userId = (int)HttpContext.Items["UserId"]!;

        var result = await _documentService.UpdateDocument(id, userId, document.Text);

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
}