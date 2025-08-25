using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class createhruser23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "CreatedAt", "Email", "Name" },
                values: new object[] { new DateTime(2025, 8, 25, 12, 0, 0, 0, DateTimeKind.Unspecified), "hr@example.com", "Ahmed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "CreatedAt", "Email", "Name" },
                values: new object[] { new DateTime(2025, 8, 25, 12, 31, 4, 876, DateTimeKind.Utc).AddTicks(7452), "admin@example.com", "ahmed" });
        }
    }
}
