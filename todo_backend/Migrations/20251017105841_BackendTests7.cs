using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class BackendTests7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JoinCode",
                table: "TimelineActivities",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimelineActivities_JoinCode",
                table: "TimelineActivities",
                column: "JoinCode",
                unique: true,
                filter: "[JoinCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimelineActivities_JoinCode",
                table: "TimelineActivities");

            migrationBuilder.DropColumn(
                name: "JoinCode",
                table: "TimelineActivities");
        }
    }
}
