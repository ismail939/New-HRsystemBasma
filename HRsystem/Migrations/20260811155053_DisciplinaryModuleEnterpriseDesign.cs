using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class DisciplinaryModuleEnterpriseDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PenaltyRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyEscalations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PenaltyRuleId = table.Column<int>(type: "int", nullable: false),
                    OccurrenceCount = table.Column<int>(type: "int", nullable: false),
                    DeductionUnit = table.Column<int>(type: "int", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeductionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsWarning = table.Column<bool>(type: "bit", nullable: false),
                    ActionRequired = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyEscalations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyEscalations_PenaltyRules_PenaltyRuleId",
                        column: x => x.PenaltyRuleId,
                        principalTable: "PenaltyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PenaltyRuleId = table.Column<int>(type: "int", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false),
                    FromValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    DeductionUnit = table.Column<int>(type: "int", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeductionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsWarning = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenaltyLevels_PenaltyRules_PenaltyRuleId",
                        column: x => x.PenaltyRuleId,
                        principalTable: "PenaltyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PenaltyRuleId = table.Column<int>(type: "int", nullable: false),
                    PenaltyLevelId = table.Column<int>(type: "int", nullable: true),
                    PenaltyEscalationId = table.Column<int>(type: "int", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeductionUnit = table.Column<int>(type: "int", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ManagerNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PayrollItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePenalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePenalties_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePenalties_PenaltyEscalations_PenaltyEscalationId",
                        column: x => x.PenaltyEscalationId,
                        principalTable: "PenaltyEscalations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePenalties_PenaltyLevels_PenaltyLevelId",
                        column: x => x.PenaltyLevelId,
                        principalTable: "PenaltyLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePenalties_PenaltyRules_PenaltyRuleId",
                        column: x => x.PenaltyRuleId,
                        principalTable: "PenaltyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePenalties_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_ApprovedByUserId",
                table: "EmployeePenalties",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_EmployeeId_IncidentDate",
                table: "EmployeePenalties",
                columns: new[] { "EmployeeId", "IncidentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_EmployeeId_Status",
                table: "EmployeePenalties",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_PenaltyEscalationId",
                table: "EmployeePenalties",
                column: "PenaltyEscalationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_PenaltyLevelId",
                table: "EmployeePenalties",
                column: "PenaltyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePenalties_PenaltyRuleId",
                table: "EmployeePenalties",
                column: "PenaltyRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyEscalations_PenaltyRuleId_OccurrenceCount",
                table: "PenaltyEscalations",
                columns: new[] { "PenaltyRuleId", "OccurrenceCount" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyLevels_PenaltyRuleId_SequenceOrder",
                table: "PenaltyLevels",
                columns: new[] { "PenaltyRuleId", "SequenceOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyRules_Code",
                table: "PenaltyRules",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeePenalties");

            migrationBuilder.DropTable(
                name: "PenaltyEscalations");

            migrationBuilder.DropTable(
                name: "PenaltyLevels");

            migrationBuilder.DropTable(
                name: "PenaltyRules");
        }
    }
}
