using aresu_txt_editor_backend.Data;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace aresu_txt_editor_backend.Services;

public class DocumentService(MssqlDbContext _dbContext, ILogger<DocumentService> _logger) : IDocumentService
{
    public async Task CreateNewDocument(CreateDocumentDto newDocument, int userId)
    {
        try
        {
            await _dbContext.TextDocuments.AddAsync(new TextDocument()
            {
                Title = newDocument.Title,
                Content = string.Empty,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create document.");
            throw;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<DocumentBriefDto>> GetUserDocuments(int userId)
    {
        try
        {
            var documents = _dbContext.TextDocuments
                .Where(x => x.UserId == userId)
                .Select((document) => new DocumentBriefDto
                {
                    Id = document.Id,
                    Title = document.Title,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt,
                });

            return await documents.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get documents for user with id {UserId}.", userId);
        }

        return [];
    }

    public async Task<bool> UpdateDocument(int documentId, int userId, string newContent)
    {
        try
        {
            var rowsAffected = await _dbContext.TextDocuments
                .Where(doc => doc.UserId == userId && doc.Id == documentId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(doc => doc.Content, newContent));

            if (rowsAffected == 0)
                return false;
        
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update contents of document with id {DocumentId} for user with id {UserId}.", documentId, userId);
            throw;
        }
    }
}