using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class update123sd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LeaveRequestHistory",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId1",
                table: "Departments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestHistory_UserId",
                table: "LeaveRequestHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId1",
                table: "Departments",
                column: "ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ManagerId1",
                table: "Departments",
                column: "ManagerId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequestHistory_Users_UserId",
                table: "LeaveRequestHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ManagerId1",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequestHistory_Users_UserId",
                table: "LeaveRequestHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequestHistory_UserId",
                table: "LeaveRequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId1",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LeaveRequestHistory");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
