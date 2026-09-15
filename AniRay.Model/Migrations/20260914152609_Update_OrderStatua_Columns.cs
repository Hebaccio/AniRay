using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniRay.Model.Migrations
{
    /// <inheritdoc />
    public partial class Update_OrderStatua_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserOnly",
                table: "OrderStatuses",
                newName: "ForUsers");

            migrationBuilder.RenameColumn(
                name: "EmployeeOnly",
                table: "OrderStatuses",
                newName: "ForEmployees");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ForEmployees", "ForUsers" },
                values: new object[] { true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForUsers",
                table: "OrderStatuses",
                newName: "UserOnly");

            migrationBuilder.RenameColumn(
                name: "ForEmployees",
                table: "OrderStatuses",
                newName: "EmployeeOnly");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EmployeeOnly", "UserOnly" },
                values: new object[] { false, false });
        }
    }
}
