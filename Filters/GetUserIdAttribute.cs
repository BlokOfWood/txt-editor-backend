using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace aresu_txt_editor_backend.Filters;

public class GetUserIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userIdString = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null || !int.TryParse(userIdString, out int userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["UserId"] = userId;
    }
}