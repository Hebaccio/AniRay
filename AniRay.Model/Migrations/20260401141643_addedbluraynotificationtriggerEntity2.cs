using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class addedbluraynotificationtriggerEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BluRayNotificationTriggers",
                columns: table => new
                {
                    Key = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BluRayId = table.Column<int>(type: "int", nullable: false),
                    Trigger = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BluRayNotificationTriggers", x => x.Key);
                    table.ForeignKey(
                        name: "FK_BluRayNotificationTriggers_BluRays_BluRayId",
                        column: x => x.BluRayId,
                        principalTable: "BluRays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BluRayNotificationTriggers_BluRayId",
                table: "BluRayNotificationTriggers",
                column: "BluRayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BluRayNotificationTriggers");
        }
    }
}
