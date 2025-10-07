using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.CreateTable(
                name: "ActivityStorage",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityStorage", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_ActivityStorage_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_ActivityStorage_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityStorage_CategoryId",
                table: "ActivityStorage",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityStorage_UserId",
                table: "ActivityStorage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "TimelineActivities",
                principalColumn: "ActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.DropTable(
                name: "ActivityStorage");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "TimelineActivities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
