using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixedDefaultOnTextDocumentContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "TextDocuments");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "TextDocuments",
                type: "varbinary(max)",
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "InitializationVector",
                table: "TextDocuments",
                type: "varbinary(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "TextDocuments");

            migrationBuilder.DropColumn(
                name: "InitializationVector",
                table: "TextDocuments");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "TextDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
