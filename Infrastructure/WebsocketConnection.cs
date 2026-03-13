using System.Net.WebSockets;
using aresu_txt_editor_backend.Models.Messages;

namespace aresu_txt_editor_backend.Infrastructure;

public class WebSocketConnection(WebSocket webSocket)
{
    private readonly WebSocket ws = webSocket;

    public async Task SendAsync(MessageBase message, CancellationToken? cancellationToken)
    {
        await ws.SendAsync(
            new ArraySegment<byte>(message.GetMessageBytes()), 
            WebSocketMessageType.Binary, 
            true, 
            cancellationToken ?? CancellationToken.None);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken? cancellationToken)
    {
        return await ws.ReceiveAsync(
            new ArraySegment<byte>(buffer), 
            cancellationToken ?? CancellationToken.None);
    }
}