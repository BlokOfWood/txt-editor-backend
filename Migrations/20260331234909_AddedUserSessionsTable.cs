using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aresu_txt_editor_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserSessionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TextDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => new { x.SessionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserSessions_TextDocuments_TextDocumentId",
                        column: x => x.TextDocumentId,
                        principalTable: "TextDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_TextDocumentId",
                table: "UserSessions",
                column: "TextDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessions");
        }
    }
}
