using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class departmentase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Departments",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Departments");
        }
    }
}
