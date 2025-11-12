using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewBeginnings1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.DropTable(
                name: "TimelineRecurrenceExceptions");

            migrationBuilder.DropTable(
                name: "TimelineRecurrenceInstances");

            migrationBuilder.DropTable(
                name: "TimelineActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                column: "ActivityId");

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JoinCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activities_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ActivityRecurrenceRules",
                columns: table => new
                {
                    RecurrenceRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysOfMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateRangeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRangeEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecurrenceRules", x => x.RecurrenceRuleId);
                    table.ForeignKey(
                        name: "FK_ActivityRecurrenceRules_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId");
                });

            migrationBuilder.CreateTable(
                name: "ActivityInstances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    RecurrenceRuleId = table.Column<int>(type: "int", nullable: true),
                    OccurrenceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DidOccur = table.Column<bool>(type: "bit", nullable: false),
                    IsException = table.Column<bool>(type: "bit", nullable: false),
                    ActivityRecurrenceRuleRecurrenceRuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityInstances", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_ActivityInstances_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId");
                    table.ForeignKey(
                        name: "FK_ActivityInstances_ActivityRecurrenceRules_ActivityRecurrenceRuleRecurrenceRuleId",
                        column: x => x.ActivityRecurrenceRuleRecurrenceRuleId,
                        principalTable: "ActivityRecurrenceRules",
                        principalColumn: "RecurrenceRuleId");
                    table.ForeignKey(
                        name: "FK_ActivityInstances_ActivityRecurrenceRules_RecurrenceRuleId",
                        column: x => x.RecurrenceRuleId,
                        principalTable: "ActivityRecurrenceRules",
                        principalColumn: "RecurrenceRuleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CategoryId",
                table: "Activities",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_JoinCode",
                table: "Activities",
                column: "JoinCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OwnerId",
                table: "Activities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstances_ActivityId",
                table: "ActivityInstances",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstances_ActivityRecurrenceRuleRecurrenceRuleId",
                table: "ActivityInstances",
                column: "ActivityRecurrenceRuleRecurrenceRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstances_RecurrenceRuleId",
                table: "ActivityInstances",
                column: "RecurrenceRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecurrenceRules_ActivityId",
                table: "ActivityRecurrenceRules",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityMembers_Activities_ActivityId",
                table: "ActivityMembers");

            migrationBuilder.DropTable(
                name: "ActivityInstances");

            migrationBuilder.DropTable(
                name: "ActivityRecurrenceRules");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityMembers",
                table: "ActivityMembers",
                columns: new[] { "ActivityId", "UserId" });

            migrationBuilder.CreateTable(
                name: "TimelineActivities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    End_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsManuallyFinished = table.Column<bool>(type: "bit", nullable: false),
                    Is_recurring = table.Column<bool>(type: "bit", nullable: false),
                    JoinCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    PlannedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Recurrence_rule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineActivities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_TimelineActivities_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TimelineActivities_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TimelineRecurrenceExceptions",
                columns: table => new
                {
                    ExceptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    ExceptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    NewDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    NewStartTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineRecurrenceExceptions", x => x.ExceptionId);
                    table.ForeignKey(
                        name: "FK_TimelineRecurrenceExceptions_TimelineActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "TimelineActivities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimelineRecurrenceInstances",
                columns: table => new
                {
                    InstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    OccurrenceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineRecurrenceInstances", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_TimelineRecurrenceInstances_TimelineActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "TimelineActivities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimelineActivities_CategoryId",
                table: "TimelineActivities",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineActivities_JoinCode",
                table: "TimelineActivities",
                column: "JoinCode",
                unique: true,
                filter: "[JoinCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineActivities_OwnerId",
                table: "TimelineActivities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineRecurrenceExceptions_ActivityId",
                table: "TimelineRecurrenceExceptions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineRecurrenceInstances_ActivityId",
                table: "TimelineRecurrenceInstances",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityMembers_TimelineActivities_ActivityId",
                table: "ActivityMembers",
                column: "ActivityId",
                principalTable: "TimelineActivities",
                principalColumn: "ActivityId");
        }
    }
}
