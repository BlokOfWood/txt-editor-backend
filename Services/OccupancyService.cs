using System.Net.WebSockets;
using aresu_txt_editor_backend.Interfaces;

namespace aresu_txt_editor_backend.Services;

public class OccupancyService(ILogger<OccupancyService> _logger, IHostApplicationLifetime _appLifetime) : IOccupancyService
{

    public async Task NewSessionAsync(WebSocket newWsSession)
    {

        var buffer = new byte[1024 * 4];
        var receiveResult = await newWsSession.ReceiveAsync(
            new ArraySegment<byte>(buffer), _appLifetime.ApplicationStopping);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await newWsSession.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await newWsSession.ReceiveAsync(
                new ArraySegment<byte>(buffer), _appLifetime.ApplicationStopping);

            _logger.LogInformation("{}", System.Text.Encoding.UTF8.GetString(buffer));
        }

    }
}