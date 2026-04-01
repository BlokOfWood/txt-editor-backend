using System.Security.Claims;
using aresu_txt_editor_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace aresu_txt_editor_backend.Filters;

public class ValidateSessionIdAttribute(IOccupancyService _occupancyService) : GetSessionIdAttribute 
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string? sessionIdString = context.HttpContext.Request.Headers["SessionId"].FirstOrDefault();

        if (sessionIdString is null || !long.TryParse(sessionIdString, out long sessionId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["SessionId"] = sessionId;

        var userIdString = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null || !int.TryParse(userIdString, out int userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var documentIdString = (string)(context.HttpContext.GetRouteValue("id") ?? throw new Exception("No document id found in url."));
        if (documentIdString == null || !int.TryParse(documentIdString, out int documentId))
        {
            context.Result = new BadRequestResult();
            return;
        }

        var isDocumentOccupiedResult = await _occupancyService.IsDocumentOccupiedAsync(userId, (long)context.HttpContext.Items["SessionId"]!, documentId);

        switch (isDocumentOccupiedResult)
        {
            case DocumentLockOpResult.INVALID_SESSION_ID:
                context.Result = new UnauthorizedResult();
                return;
            case DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION:
                context.Result = new ForbidResult();
                return;
        }

        context.HttpContext.Items["SessionState"] = isDocumentOccupiedResult;

        await next();
    }
}