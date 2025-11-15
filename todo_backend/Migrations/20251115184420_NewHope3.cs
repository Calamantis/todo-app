using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewHope3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ActivityInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstances_UserId",
                table: "ActivityInstances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityInstances_Users_UserId",
                table: "ActivityInstances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityInstances_Users_UserId",
                table: "ActivityInstances");

            migrationBuilder.DropIndex(
                name: "IX_ActivityInstances_UserId",
                table: "ActivityInstances");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ActivityInstances");
        }
    }
}
