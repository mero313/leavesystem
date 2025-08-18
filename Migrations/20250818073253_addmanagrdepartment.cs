using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class addmanagrdepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ManagerId1",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "ManagerId1",
                table: "Departments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ManagerId1",
                table: "Departments",
                newName: "IX_Departments_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_UserId",
                table: "Departments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_UserId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Departments",
                newName: "ManagerId1");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_UserId",
                table: "Departments",
                newName: "IX_Departments_ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ManagerId1",
                table: "Departments",
                column: "ManagerId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
