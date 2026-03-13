
using aresu_txt_editor_backend.Infrastructure;

namespace aresu_txt_editor_backend.Interfaces;

public interface IOccupancyService
{
    public Task NewSessionAsync(int userId, WebSocketConnection newWsSession);
    public DocumentLockOpResult TryOccupyDocument(int userId, long sessionId, int documentId);
    public DocumentLockOpResult TryRemoveDocumentLock(int userId, long sessionId, int documentId);
    public DocumentLockOpResult IsDocumentOccupied(int userId, long sessionId, int documentId);
}