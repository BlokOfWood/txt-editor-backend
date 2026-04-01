
using aresu_txt_editor_backend.Infrastructure;

namespace aresu_txt_editor_backend.Interfaces;

public interface IOccupancyService
{
    public Task NewSessionAsync(int userId, WebSocketConnection newWsSession);
    public Task<DocumentLockOpResult> TryOccupyDocumentAsync(int userId, long sessionId, int documentId);
    public Task<DocumentLockOpResult> TryClearUserOccupancyAsync(int userId, long sessionId);
    public Task<DocumentLockOpResult> IsDocumentOccupiedAsync(int userId, long sessionId, int documentId);
}