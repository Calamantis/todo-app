using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class CoreTests1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                columns: new[] { "ActivityId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                column: "ActivityId");
        }
    }
}
