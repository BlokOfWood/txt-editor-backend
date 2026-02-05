using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class RenamedContentToEncryptedContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "TextDocuments",
                newName: "EncryptedContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedContent",
                table: "TextDocuments",
                newName: "Content");
        }
    }
}
