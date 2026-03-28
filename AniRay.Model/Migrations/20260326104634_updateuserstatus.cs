using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class updateuserstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StatusForEmployee",
                table: "UserStatuses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StatusForUser",
                table: "UserStatuses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "StatusForEmployee", "StatusForUser" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "StatusForEmployee", "StatusForUser" },
                values: new object[] { false, true });

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "StatusForEmployee", "StatusForUser" },
                values: new object[] { false, true });

            migrationBuilder.InsertData(
                table: "UserStatuses",
                columns: new[] { "Id", "IsDeleted", "Name", "StatusForEmployee", "StatusForUser" },
                values: new object[] { 4, false, "Fired Or Quit", true, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "StatusForEmployee",
                table: "UserStatuses");

            migrationBuilder.DropColumn(
                name: "StatusForUser",
                table: "UserStatuses");
        }
    }
}
