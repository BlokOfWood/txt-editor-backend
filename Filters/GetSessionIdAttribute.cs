using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace aresu_txt_editor_backend.Filters;

public class GetSessionIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string? sessionIdString = context.HttpContext.Request.Headers["SessionId"].FirstOrDefault();

        if (sessionIdString is null || !long.TryParse(sessionIdString, out long sessionId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["SessionId"] = sessionId;
    }
}