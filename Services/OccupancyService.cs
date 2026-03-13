using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using aresu_txt_editor_backend.Infrastructure;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models.Messages;

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

    public async Task NewSessionAsync(int userId, WebSocketConnection newWsSession)
    {
        var buffer = new byte[1024 * 4];

        byte[] newSessionIdBytes = RandomNumberGenerator.GetBytes(8);
        while (sessionStateLookup.ContainsKey(BitConverter.ToInt64(newSessionIdBytes)))
            newSessionIdBytes = RandomNumberGenerator.GetBytes(8);

        AssignSessionIdMessage assignSessionIdMessage = new(newSessionIdBytes);

        sessionStateLookup[assignSessionIdMessage.SessionId] = (userId, null);

        await newWsSession.SendAsync(
            assignSessionIdMessage,
            appLifetime.ApplicationStopping);

        WebSocketReceiveResult receiveResult;
        do
        {
            receiveResult = await newWsSession.ReceiveAsync(buffer, appLifetime.ApplicationStopping);
        }
        while (!receiveResult.CloseStatus.HasValue);

        if (isAppStopping) return;

        if (sessionStateLookup.TryRemove(assignSessionIdMessage.SessionId, out var sessionState) && sessionState.documentId is not null)
        {
            documentStateLookup.TryRemove((int)sessionState.documentId, out var _);
        }
    }

    public DocumentLockOpResult TryOccupyDocument(int userId, long sessionId, int documentId)
    {
        if (!sessionStateLookup.TryGetValue(sessionId, out var occupiedDocumentEntry))
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (occupiedDocumentEntry.userId != userId)
            return DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH;

        if (documentStateLookup.TryGetValue(documentId, out var occupyingSessionId) && occupyingSessionId != sessionId)
            return DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;

        if (occupiedDocumentEntry.documentId != documentId)
        {
            if (occupiedDocumentEntry.documentId is not null)
            {
                documentStateLookup.TryRemove(documentId, out var _);
            }

            sessionStateLookup[sessionId] = (userId, documentId);
            documentStateLookup[documentId] = sessionId;
        }

        return DocumentLockOpResult.SUCCESS;
    }

    public DocumentLockOpResult TryClearUserOccupancy(int userId, long sessionId)
    {
        if (!sessionStateLookup.TryGetValue(sessionId, out var occupiedDocumentEntry))
            return DocumentLockOpResult.INVALID_SESSION_ID;

        if (occupiedDocumentEntry.userId != userId)
            return DocumentLockOpResult.SESSION_ID_OWNER_DOES_NOT_MATCH;

        if (occupiedDocumentEntry.documentId is null)
            return DocumentLockOpResult.NO_DOCUMENT_OCCUPIED_BY_SESSION;

        if (!documentStateLookup.TryGetValue((int)occupiedDocumentEntry.documentId, out var documentState))
            return DocumentLockOpResult.DOCUMENT_UNOCCUPIED;

        if (documentState != sessionId)
            return DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;

        documentStateLookup.TryRemove((int)occupiedDocumentEntry.documentId, out var _);
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

        return occupyingSessionId == sessionId ? DocumentLockOpResult.DOCUMENT_OCCUPIED_BY_SESSION : DocumentLockOpResult.DOCUMENT_NOT_OCCUPIED_BY_SESSION;
    }
}