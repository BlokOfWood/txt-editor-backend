using aresu_txt_editor_backend.Models.Enums;

namespace aresu_txt_editor_backend.Models.Messages;

public class AssignSessionIdMessage(byte[] sessionIdBytes) : MessageBase(OccupancyWebsocketMessageType.ASSIGN_SESSION_ID, sessionIdBytes)
{
    public long SessionId { get; private init; } = BitConverter.ToInt64(sessionIdBytes);
}