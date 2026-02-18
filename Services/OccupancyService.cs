using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using aresu_txt_editor_backend.Interfaces;
using Microsoft.VisualBasic;

namespace aresu_txt_editor_backend.Services;

public class OccupancyService : IOccupancyService
{
    // Key is SessionId 
    private readonly ConcurrentDictionary<long, (int userId, int? documentId)> sessionStateLookup = [];
    // Key is DocumentId, Value is SessionId
    private readonly ConcurrentDictionary<int, long> documentStateLookup = [];
    private readonly ILogger<OccupancyService> logger;
    private readonly IHostApplicationLifetime appLifetime;
    bool isAppStopping = false;

    public OccupancyService(ILogger<OccupancyService> _logger, IHostApplicationLifetime _appLifetime)
    {
        logger = _logger;
        appLifetime = _appLifetime;
        appLifetime.ApplicationStopping.Register(() => isAppStopping = true);
    }

    public async Task NewSessionAsync(int userId, WebSocket newWsSession)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await newWsSession.ReceiveAsync(
            new ArraySegment<byte>(buffer), appLifetime.ApplicationStopping);

        byte[] newSessionIdBytes = RandomNumberGenerator.GetBytes(8);
        while (sessionStateLookup.ContainsKey(BitConverter.ToInt64(newSessionIdBytes)))
            newSessionIdBytes = RandomNumberGenerator.GetBytes(8);

        long newSessionId = BitConverter.ToInt64(newSessionIdBytes);

        sessionStateLookup[newSessionId] = (userId, null);

        await newWsSession.SendAsync(new ArraySegment<byte>(newSessionIdBytes), WebSocketMessageType.Binary, true, appLifetime.ApplicationStopping);

        while (!receiveResult.CloseStatus.HasValue)
        {
            receiveResult = await newWsSession.ReceiveAsync(
                new ArraySegment<byte>(buffer), appLifetime.ApplicationStopping);

            logger.LogInformation("{}", System.Text.Encoding.UTF8.GetString(buffer));
        }

        if (isAppStopping) return;

        if (sessionStateLookup.TryRemove(newSessionId, out var sessionState))
        {
            if (sessionState.documentId is not null)
                documentStateLookup.TryRemove((int)sessionState.documentId, out var _);
        }
    }

    public DocumentLockOpResult TryOccupyDocument(int userId, long sessionId, int documentId)
    {
        if (!sessionStateLookup.TryGetValue(sessionId, out var occupiedDocumentEntry))
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (occupiedDocumentEntry.userId != userId)
            return DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH;

        if (documentStateLookup.ContainsKey(documentId))
            return DocumentLockOpResult.DOCUMENT_OCCUPIED;

        if (occupiedDocumentEntry.documentId is not null)
            documentStateLookup.TryRemove(documentId, out var _);

        sessionStateLookup[sessionId] = (userId, documentId);
        documentStateLookup[documentId] = sessionId;

        return DocumentLockOpResult.SUCCESS;
    }

    public DocumentLockOpResult TryRemoveDocumentLock(int userId, long sessionId, int documentId)
    {
        if (!sessionStateLookup.TryGetValue(sessionId, out var occupiedDocumentEntry))
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (occupiedDocumentEntry.userId != userId)
            return DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH;

        if (!documentStateLookup.TryGetValue(documentId, out var documentState))
            return DocumentLockOpResult.DOCUMENT_UNOCCUPIED;

        if (documentState != sessionId)
            return DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;

        documentStateLookup.TryRemove(documentId, out var _);
        sessionStateLookup[sessionId] = (userId, null);

        return DocumentLockOpResult.SUCCESS;
    }

    public DocumentLockOpResult IsDocumentOccupied(int userId, long sessionId, int documentId)
    {
        if (!sessionStateLookup.TryGetValue(sessionId, out var occupiedDocumentEntry))
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (occupiedDocumentEntry.userId != userId)
            return DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH;

        if (!documentStateLookup.TryGetValue(documentId, out var occupyingSessionId))
            return DocumentLockOpResult.DOCUMENT_UNOCCUPIED;

        return occupyingSessionId == documentId ? DocumentLockOpResult.DOCUMENT_OCCUPIED_BY_SESSION : DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;
    }
}