
using System.Net.WebSockets;

namespace aresu_txt_editor_backend.Interfaces;

public interface IOccupancyService
{
    public Task NewSessionAsync(int userId, WebSocket newWsSession);
    public DocumentLockOpResult TryOccupyDocument(int userId, long sessionId, int documentId);
    public DocumentLockOpResult TryRemoveDocumentLock(int userId, long sessionId, int documentId);
    public DocumentLockOpResult IsDocumentOccupiedBySessionId(int userId, long sessionId, int documentId);
}