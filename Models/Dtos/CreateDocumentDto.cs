using System.ComponentModel.DataAnnotations;

namespace aresu_txt_editor_backend.Models.Dtos;

public class CreateDocumentDto
{
    [StringLength(255)]
    public required string Title { get; init; }
    public required byte[] Content { get; init; }
    [MaxLength(12)]
    public required byte[] InitializationVector { get; init; }
}