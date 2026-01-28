using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeTitleUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_Title_UserId",
                table: "TextDocuments",
                columns: new[] { "Title", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_Title_UserId",
                table: "TextDocuments");
        }
    }
}
