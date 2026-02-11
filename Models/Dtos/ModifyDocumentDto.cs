using System.ComponentModel.DataAnnotations;

namespace aresu_txt_editor_backend.Models.Dtos;

public class ModifyDocumentDto
{
    [StringLength(255)]
    public string? Title { get; init; }
    public byte[]? Content { get; init; }
    [MaxLength(12)]
    public byte[]? InitializationVector { get; init; }
}