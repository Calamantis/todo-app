using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class BackendTests11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsManuallyFinished",
                table: "TimelineActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlannedDurationMinutes",
                table: "TimelineActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManuallyFinished",
                table: "TimelineActivities");

            migrationBuilder.DropColumn(
                name: "PlannedDurationMinutes",
                table: "TimelineActivities");
        }
    }
}
