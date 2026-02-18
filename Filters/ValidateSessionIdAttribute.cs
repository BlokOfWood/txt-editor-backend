using System.Security.Claims;
using aresu_txt_editor_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace aresu_txt_editor_backend.Filters;

public class ValidateSessionIdAttribute(IOccupancyService _occupancyService) : GetSessionIdAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userIdString = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdString == null || !int.TryParse(userIdString, out int userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);

        var documentId = (int)(context.HttpContext.GetRouteValue("id") ?? throw new Exception("No document id found in url."));
        var isDocumentOccupiedResult = _occupancyService.IsDocumentOccupied(userId, (long)context.HttpContext.Items["SessionId"]!, documentId);

        switch (isDocumentOccupiedResult)
        {
            case DocumentLockOpResult.INVALID_SESSION_ID or DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH:
                context.Result = new UnauthorizedResult();
                return;
            case DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION:
                context.Result = new ForbidResult();
                return;
        }

        context.HttpContext.Items["SessionState"] = isDocumentOccupiedResult;
    }
}