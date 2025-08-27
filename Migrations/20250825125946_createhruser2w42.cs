using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class createhruser2w42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Email", "IsActive", "ManagerId", "Name", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { -2, new DateTime(2025, 8, 25, 12, 0, 0, 0, DateTimeKind.Utc), null, "hr@example.com", true, null, "Ahmed", "123", 0, new DateTime(2025, 8, 25, 12, 0, 0, 0, DateTimeKind.Utc), "hr" });
        }
    }
}
