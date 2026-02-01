namespace aresu_txt_editor_backend.Models;

public class DocumentBrief
{
    public int Id { get; set; }
    public required string? Title { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}