using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class createhruser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Email", "IsActive", "ManagerId", "Name", "PasswordHash", "Role", "Username" },
                values: new object[] { -2, new DateTime(2025, 8, 25, 12, 31, 4, 876, DateTimeKind.Utc).AddTicks(7452), null, "admin@example.com", true, null, "ahmed", "123", 0, "hr" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Email", "IsActive", "ManagerId", "Name", "PasswordHash", "Role", "Username" },
                values: new object[] { 2, new DateTime(2025, 8, 25, 12, 16, 38, 268, DateTimeKind.Utc).AddTicks(4199), null, "admin@example.com", true, null, "ahmed", "123", 0, "hr" });
        }
    }
}
