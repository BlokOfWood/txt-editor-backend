
namespace aresu_txt_editor_backend.Models.Messages;

public class DocumentUnoccupationMessage(int documentId)
: MessageBase(
    Enums.OccupancyWebsocketMessageType.REPORT_FREED_DOCUMENT,
    BitConverter.GetBytes(documentId))
{
    public int DocumentId = documentId;
}