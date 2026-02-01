namespace aresu_txt_editor_backend.Models.Dtos;

public class DocumentBriefsDto
{
    public required IReadOnlyList<DocumentBrief> DocumentBriefs { get; set; }
    public required int TotalDocumentCount { get; set; }
}