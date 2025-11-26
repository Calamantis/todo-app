using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class AlmostDone1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllowMentions",
                table: "Users",
                newName: "AllowTimeline");

            migrationBuilder.AddColumn<bool>(
                name: "isFriendsOnly",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isFriendsOnly",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "AllowTimeline",
                table: "Users",
                newName: "AllowMentions");
        }
    }
}
