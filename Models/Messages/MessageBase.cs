using aresu_txt_editor_backend.Models.Enums;

namespace aresu_txt_editor_backend.Models.Messages;

public abstract class MessageBase
{
    public OccupancyWebsocketMessageType MessageType { get; private init; } 
    private byte[] MessageBytes { get; init; }

    protected MessageBase(OccupancyWebsocketMessageType messageType, byte[] messageBytes)
    {
        MessageType = messageType;
        MessageBytes = messageBytes;
    }

    public virtual byte[] GetMessageBytes()
    {
        return [(byte)MessageType, ..MessageBytes];
    }
}