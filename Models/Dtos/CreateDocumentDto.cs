namespace aresu_txt_editor_backend.Models.Dtos;

public class CreateDocumentDto
{
    public required string Title { get; init; }
    public required byte[] Content { get; init; }
    public required byte[] InitializationVector { get; init; }
}