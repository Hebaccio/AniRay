using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class UserBluRayNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBluRayNotifications",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BluRayId = table.Column<int>(type: "int", nullable: false),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBluRayNotifications", x => new { x.UserId, x.BluRayId });
                    table.ForeignKey(
                        name: "FK_UserBluRayNotifications_BluRays_BluRayId",
                        column: x => x.BluRayId,
                        principalTable: "BluRays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBluRayNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBluRayNotifications_BluRayId",
                table: "UserBluRayNotifications",
                column: "BluRayId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBluRayNotifications_UserId_BluRayId_EmailSent",
                table: "UserBluRayNotifications",
                columns: new[] { "UserId", "BluRayId", "EmailSent" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBluRayNotifications");
        }
    }
}
