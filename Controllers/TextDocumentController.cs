using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aresu_txt_editor_backend.Controllers;

[ApiController]
[Route("document")]
public class TextDocumentController : ControllerBase
{
   [HttpGet]
   [Authorize]
   public IActionResult GetDocumentTitles() 
   {
      return Ok("TextDocumentController is working!");
   }
}