using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewHope2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_Users_UserId",
                table: "ActivityMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                columns: new[] { "ActivityId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_Users_UserId",
                table: "ActivityMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_Users_UserId",
                table: "ActivityMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_Users_UserId",
                table: "ActivityMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
