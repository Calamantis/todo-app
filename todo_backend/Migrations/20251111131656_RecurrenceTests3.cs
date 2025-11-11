using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class RecurrenceTests3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdditional",
                table: "TimelineRecurrenceExceptions");

            migrationBuilder.DropColumn(
                name: "IsSkipped",
                table: "TimelineRecurrenceExceptions");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "TimelineRecurrenceExceptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "TimelineRecurrenceExceptions");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditional",
                table: "TimelineRecurrenceExceptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSkipped",
                table: "TimelineRecurrenceExceptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
