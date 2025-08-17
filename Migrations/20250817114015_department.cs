using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class department : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequestHistories_LeaveRequests_LeaveRequestId",
                table: "LeaveRequestHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequestHistories_Users_ActionByUserId",
                table: "LeaveRequestHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequestHistories",
                table: "LeaveRequestHistories");

            migrationBuilder.RenameTable(
                name: "LeaveRequestHistories",
                newName: "LeaveRequestHistory");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequestHistories_LeaveRequestId",
                table: "LeaveRequestHistory",
                newName: "IX_LeaveRequestHistory_LeaveRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequestHistories_ActionByUserId",
                table: "LeaveRequestHistory",
                newName: "IX_LeaveRequestHistory_ActionByUserId");

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequestHistory",
                table: "LeaveRequestHistory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequestHistory_LeaveRequests_LeaveRequestId",
                table: "LeaveRequestHistory",
                column: "LeaveRequestId",
                principalTable: "LeaveRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequestHistory_Users_ActionByUserId",
                table: "LeaveRequestHistory",
                column: "ActionByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequestHistory_LeaveRequests_LeaveRequestId",
                table: "LeaveRequestHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequestHistory_Users_ActionByUserId",
                table: "LeaveRequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequestHistory",
                table: "LeaveRequestHistory");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Departments");

            migrationBuilder.RenameTable(
                name: "LeaveRequestHistory",
                newName: "LeaveRequestHistories");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequestHistory_LeaveRequestId",
                table: "LeaveRequestHistories",
                newName: "IX_LeaveRequestHistories_LeaveRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaveRequestHistory_ActionByUserId",
                table: "LeaveRequestHistories",
                newName: "IX_LeaveRequestHistories_ActionByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequestHistories",
                table: "LeaveRequestHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequestHistories_LeaveRequests_LeaveRequestId",
                table: "LeaveRequestHistories",
                column: "LeaveRequestId",
                principalTable: "LeaveRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequestHistories_Users_ActionByUserId",
                table: "LeaveRequestHistories",
                column: "ActionByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
