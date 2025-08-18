using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class departmentaseddssdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments");
        }
    }
}
