using aresu_txt_editor_backend.Models.Enums;

namespace aresu_txt_editor_backend.Models.Messages;

public class AssignSessionIdMessage : MessageBase
{
    public long SessionId { get; private init; }

    public AssignSessionIdMessage(byte[] sessionIdBytes) : base(OccupancyWebsocketMessageType.ASSIGN_SESSION_ID, sessionIdBytes)
    {
        SessionId = BitConverter.ToInt64(sessionIdBytes);
    }
}