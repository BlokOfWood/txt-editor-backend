using System.ComponentModel.DataAnnotations;

namespace aresu_txt_editor_backend.Models;

public class DocumentContentDto
{
    [StringLength(255)]
    public required string Title { get; init; }
    public required byte[] Content { get; init; }
    [MaxLength(12)]
    public required byte[] InitializationVector { get; init; }
}