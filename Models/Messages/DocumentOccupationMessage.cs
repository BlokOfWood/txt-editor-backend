namespace aresu_txt_editor_backend.Models.Messages;

public class DocumentOccupationMessage(int documentId, long occupyingSession) 
: MessageBase(
    Enums.OccupancyWebsocketMessageType.REPORT_OCCUPIED_DOCUMENT, 
    [.. BitConverter.GetBytes(documentId), .. BitConverter.GetBytes(occupyingSession)])
{
    public int DocumentId = documentId;
    public long OccupyingSession = occupyingSession;
}