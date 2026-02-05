namespace aresu_txt_editor_backend.Models.Dtos;

public class ModifyDocumentDto
{
    public string? Title { get; init; }
    public byte[]? Content { get; init; }
    public byte[]? InitializationVector { get; init; }
}