using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedTextDocument1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "TextDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TextDocuments_Id_UserId",
                table: "TextDocuments",
                columns: new[] { "Id", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextDocuments_Id_UserId",
                table: "TextDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "TextDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");
        }
    }
}
