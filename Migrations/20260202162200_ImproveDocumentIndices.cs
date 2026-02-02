using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class ImproveDocumentIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_Id_UserId",
                table: "TextDocuments");

            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_Title_UserId",
                table: "TextDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_UserId_Id",
                table: "TextDocuments",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_UserId_Title",
                table: "TextDocuments",
                columns: new[] { "UserId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_UserId_Id",
                table: "TextDocuments");

            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_UserId_Title",
                table: "TextDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_Id_UserId",
                table: "TextDocuments",
                columns: new[] { "Id", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_Title_UserId",
                table: "TextDocuments",
                columns: new[] { "Title", "UserId" },
                unique: true);
        }
    }
}
