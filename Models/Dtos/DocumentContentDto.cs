namespace aresu_txt_editor_backend.Models;

public class DocumentContentDto
{
    public required string Title { get; init; }
    public required byte[] Content { get; init; }
    public required byte[] InitializationVector { get; init; }
}