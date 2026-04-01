using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class removedcolumnfromnoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBluRayNotifications_UserId_BluRayId_EmailSent",
                table: "UserBluRayNotifications");

            migrationBuilder.DropColumn(
                name: "EmailSent",
                table: "UserBluRayNotifications");

            migrationBuilder.CreateIndex(
                name: "IX_UserBluRayNotifications_UserId_BluRayId",
                table: "UserBluRayNotifications",
                columns: new[] { "UserId", "BluRayId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBluRayNotifications_UserId_BluRayId",
                table: "UserBluRayNotifications");

            migrationBuilder.AddColumn<bool>(
                name: "EmailSent",
                table: "UserBluRayNotifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserBluRayNotifications_UserId_BluRayId_EmailSent",
                table: "UserBluRayNotifications",
                columns: new[] { "UserId", "BluRayId", "EmailSent" });
        }
    }
}
