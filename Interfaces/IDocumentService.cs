using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Dtos;

namespace aresu_txt_editor_backend.Interfaces;

public interface IDocumentService
{
   Task<bool> CreateNewDocument(CreateDocumentDto newDocument, int userId); 
   Task<IReadOnlyList<DocumentBriefDto>> GetUserDocuments(int userId);
   Task<DocumentContentDto?> GetDocumentById(int documentId, int userId);
   Task<bool> UpdateDocument(int documentId, int userId, string newContent);
}