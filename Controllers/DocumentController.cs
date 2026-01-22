using System.Security.Claims;
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
        // TODO: implement paging 
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdString == null)
        {
            return Unauthorized();
        }
        
        var parseResult = int.TryParse(userIdString, out int userId);
        
        if (!parseResult)
        {
            return Unauthorized();
        }

        var documents = _documentService.GetUserDocuments(userId);
        
        return Ok(await documents);
    }

    [HttpPost()]
    [Authorize]
    public async Task<IActionResult> AddNewDocument([FromBody] CreateDocumentDto document)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null)
        {
            return Unauthorized();
        }

        var parseResult = int.TryParse(userIdString, out int userId);

        if (!parseResult)
        {
            return Unauthorized();
        }

        await _documentService.CreateNewDocument(document, userId);

        return Ok();
    }
}