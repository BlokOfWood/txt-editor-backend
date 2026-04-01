using Microsoft.EntityFrameworkCore;

namespace aresu_txt_editor_backend.Models;

[PrimaryKey(nameof(SessionId), nameof(UserId))]
public class UserSession
{
    public required long SessionId { get; init; }
    public required int UserId { get; init; }
    public int? TextDocumentId { get; set; }
}