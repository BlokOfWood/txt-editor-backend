using System.Net.WebSockets;
using System.Security.Cryptography;
using aresu_txt_editor_backend.Data;
using aresu_txt_editor_backend.Infrastructure;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Messages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace aresu_txt_editor_backend.Services;

public class OccupancyService : IOccupancyService
{
    private readonly ILogger<OccupancyService> logger;
    private readonly IDbContextFactory<MssqlDbContext> dbContextFactory;
    private readonly IHostApplicationLifetime appLifetime;
    bool isAppStopping = false;

    public OccupancyService(ILogger<OccupancyService> _logger, IDbContextFactory<MssqlDbContext> _dbContextFactory, IHostApplicationLifetime _appLifetime)
    {
        logger = _logger;
        dbContextFactory = _dbContextFactory;
        appLifetime = _appLifetime;
        appLifetime.ApplicationStopping.Register(() => isAppStopping = true);


        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.UserSessions.ExecuteDelete();
    }

    public async Task NewSessionAsync(int userId, WebSocketConnection newWsSession)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var buffer = new byte[1024 * 4];

        byte[] newSessionIdBytes = [];
        bool assignedSessionId = false;
        EntityEntry<UserSession>? newSession = null;

        while (!assignedSessionId)
        {
            try
            {
                newSessionIdBytes = RandomNumberGenerator.GetBytes(8);

                newSession = await dbContext.UserSessions.AddAsync(new()
                {
                    SessionId = BitConverter.ToInt64(newSessionIdBytes),
                    UserId = userId,
                    TextDocumentId = null,
                });

                await dbContext.SaveChangesAsync();

                assignedSessionId = true;
            }
            catch (DbUpdateException e) when
                (e.InnerException is SqlException sqlE && sqlE.Number is (int)MssqlErrorCode.CannotInsertDuplicateKey)
            {
                logger.LogWarning("Duplicate session id generated! Key: {}", BitConverter.ToInt64(newSessionIdBytes));
                break;
            }
            catch
            {
                break;
                throw;
            }
        }

        AssignSessionIdMessage assignSessionIdMessage = new(newSessionIdBytes);

        await newWsSession.SendAsync(
            assignSessionIdMessage,
            appLifetime.ApplicationStopping);

        WebSocketReceiveResult receiveResult;
        try
        {
            do
            {
                receiveResult = await newWsSession.ReceiveAsync(buffer, appLifetime.ApplicationStopping);
            }
            while (!receiveResult.CloseStatus.HasValue);
        }
        catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) { }
        finally
        {
            if (!isAppStopping)
            {
                dbContext.Remove(newSession!.Entity);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task<DocumentLockOpResult> TryOccupyDocumentAsync(int userId, long sessionId, int documentId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var session = await dbContext.UserSessions.FindAsync(sessionId, userId);
        if (session is null)
            return DocumentLockOpResult.INVALID_SESSION_ID;

        var sessionWithDocument = await dbContext.UserSessions.Where(session => session.TextDocumentId == documentId).FirstOrDefaultAsync();
        if (sessionWithDocument is not null && sessionWithDocument.SessionId != sessionId)
            return DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;

        if (session.TextDocumentId != documentId)
        {
            session.TextDocumentId = documentId;
            await dbContext.SaveChangesAsync();
        }

        return DocumentLockOpResult.SUCCESS;
    }

    public async Task<DocumentLockOpResult> TryClearUserOccupancyAsync(int userId, long sessionId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var session = await dbContext.UserSessions.FindAsync(sessionId, userId);
        if (session is null)
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (session.TextDocumentId is null)
            return DocumentLockOpResult.NO_DOCUMENT_OCCUPIED_BY_SESSION;

        session.TextDocumentId = null;
        await dbContext.SaveChangesAsync();

        return DocumentLockOpResult.SUCCESS;
    }

    public async Task<DocumentLockOpResult> IsDocumentOccupiedAsync(int userId, long sessionId, int documentId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var session = await dbContext.UserSessions.FindAsync(sessionId, userId);
        if (session is null)
            return DocumentLockOpResult.INVALID_SESSION_ID;

        var sessionWithDocument = await dbContext.UserSessions.Where(session => session.TextDocumentId == documentId).FirstAsync();
        if (sessionWithDocument is null)
            return DocumentLockOpResult.DOCUMENT_UNOCCUPIED;

        return sessionWithDocument.SessionId == sessionId
            ? DocumentLockOpResult.DOCUMENT_OCCUPIED_BY_SESSION
            : DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;
    }
}