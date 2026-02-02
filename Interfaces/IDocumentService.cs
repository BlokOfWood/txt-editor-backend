using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Dtos;

namespace aresu_txt_editor_backend.Interfaces;

public interface IDocumentService
{
   Task<bool> CreateNewDocument(CreateDocumentDto newDocument, int userId); 
   Task<DocumentBriefsDto> GetUserDocuments(string query, int offset, int quantity, int userId);
   Task<DocumentContentDto?> GetDocumentById(int documentId, int userId);
   Task<bool> UpdateDocument(int documentId, int userId, ModifyDocumentDto newContent);
   Task<bool> DeleteDocument(int documentId, int userId);

   #if MOCKING 
   Task AddTestDocuments(int number, int userId);
   #endif
}