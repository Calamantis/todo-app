using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewHope5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "IsAlert",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "RecurrenceRule",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "RemindTime",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "IsRecurring",
                table: "Notification",
                newName: "IsRead");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Notification",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "Notification",
                newName: "IsRecurring");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notification",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAlert",
                table: "Notification",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceRule",
                table: "Notification",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemindTime",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
