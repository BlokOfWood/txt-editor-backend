
using System.Net.WebSockets;

namespace aresu_txt_editor_backend.Interfaces;

public interface IOccupancyService
{
    public Task NewSessionAsync(WebSocket newWsSession);
}