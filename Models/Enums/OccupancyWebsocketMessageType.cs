namespace aresu_txt_editor_backend.Models.Enums;

public enum OccupancyWebsocketMessageType : byte
{
    ASSIGN_SESSION_ID = 0,
    REPORT_OCCUPIED_DOCUMENT = 1,
    REPORT_FREED_DOCUMENT = 2,
}