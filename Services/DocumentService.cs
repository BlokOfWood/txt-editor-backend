using aresu_txt_editor_backend.Data;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace aresu_txt_editor_backend.Services;

public class DocumentService(MssqlDbContext _dbContext, ILogger<DocumentService> _logger) : IDocumentService
{
    public async Task<bool> CreateNewDocument(CreateDocumentDto newDocument, int userId)
    {
        try
        {
            await _dbContext.TextDocuments.AddAsync(new TextDocument()
            {
                Title = newDocument.Title,
                Content = newDocument.Content ?? string.Empty,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e) when
            (e.InnerException is SqlException sqlE &&
            sqlE.Number is (int)MssqlErrorCode.CannotInsertDuplicateKey or (int)MssqlErrorCode.UniqueConstraintViolation)
        {
            _logger.LogDebug("User with id {UserId} tried to insert document with a title that already exists.", userId);
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create document.");
            throw;
        }

        return true;
    }

    public async Task<DocumentContentDto?> GetDocumentById(int documentId, int userId)
    {
        try
        {
            return await _dbContext.TextDocuments
            .AsNoTracking()
            .Where(doc => doc.Id == documentId && doc.UserId == userId)
            .Select((doc) => new DocumentContentDto { Content = doc.Content!, Title = doc.Title! })
            .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get document with id {Id} for user with id {UserId}", documentId, userId);
            throw;
        }
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

    public async Task<bool> DeleteDocument(int documentId, int userId)
    {
        try
        {
            var rowsAffected = await _dbContext.TextDocuments
                .Where(doc => doc.UserId == userId && doc.Id == documentId)
                .ExecuteDeleteAsync();

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