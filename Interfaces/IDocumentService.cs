using aresu_txt_editor_backend.Models.Dtos;

namespace aresu_txt_editor_backend.Interfaces;

public interface IDocumentService
{
   Task CreateNewDocument(CreateDocumentDto newDocument, int userId); 
   Task<IReadOnlyList<DocumentBriefDto>> GetUserDocuments(int userId);
}