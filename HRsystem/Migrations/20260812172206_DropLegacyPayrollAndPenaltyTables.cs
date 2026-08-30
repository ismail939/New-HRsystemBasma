using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyPayrollAndPenaltyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop all foreign keys referencing SalaryComponents.
            // FK constraint names are auto-generated in the live DB (hashed), so drop them dynamically.
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(MAX) = N'';
                SELECT @sql = @sql + 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(fk.parent_object_id) + '].[' + OBJECT_NAME(fk.parent_object_id) + '] DROP CONSTRAINT [' + fk.name + '];' + CHAR(10)
                FROM sys.foreign_keys fk
                WHERE OBJECT_NAME(fk.referenced_object_id) = 'SalaryComponents';
                EXEC sp_executesql @sql;
            ");

            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "HREmployeePenalties");

            migrationBuilder.DropTable(
                name: "SalaryHistories");

            migrationBuilder.DropTable(
                name: "SalaryComponents");

            // Drop leftover SalaryComponentId column (and any indexes on it) from the kept rollup tables.
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(MAX) = N'';
                SELECT @sql = @sql + 'DROP INDEX [' + i.name + '] ON [' + OBJECT_SCHEMA_NAME(i.object_id) + '].[' + OBJECT_NAME(i.object_id) + '];' + CHAR(10)
                FROM sys.indexes i
                JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE OBJECT_NAME(i.object_id) IN ('PayrollEarnings','PayrollDeductions') AND c.name = 'SalaryComponentId' AND i.name IS NOT NULL AND i.is_primary_key = 0;
                EXEC sp_executesql @sql;

                ALTER TABLE [dbo].[PayrollEarnings] DROP COLUMN [SalaryComponentId];
                ALTER TABLE [dbo].[PayrollDeductions] DROP COLUMN [SalaryComponentId];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollEarnings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalaryComponentId",
                table: "PayrollDeductions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HREmployeePenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PenaltyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeePenalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeePenalties_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalculationMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false),
                    IsInsurable = table.Column<bool>(type: "bit", nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryComponentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_SalaryComponents_SalaryComponentId",
                        column: x => x.SalaryComponentId,
                        principalTable: "SalaryComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryComponentId = table.Column<int>(type: "int", nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryHistories_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryHistories_SalaryComponents_SalaryComponentId",
                        column: x => x.SalaryComponentId,
                        principalTable: "SalaryComponents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEarnings_SalaryComponentId",
                table: "PayrollEarnings",
                column: "SalaryComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDeductions_SalaryComponentId",
                table: "PayrollDeductions",
                column: "SalaryComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId",
                table: "EmployeeSalaries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_SalaryComponentId",
                table: "EmployeeSalaries",
                column: "SalaryComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeePenalties_EmployeeId",
                table: "HREmployeePenalties",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistories_EmployeeId",
                table: "SalaryHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistories_SalaryComponentId",
                table: "SalaryHistories",
                column: "SalaryComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollDeductions_SalaryComponents_SalaryComponentId",
                table: "PayrollDeductions",
                column: "SalaryComponentId",
                principalTable: "SalaryComponents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollEarnings_SalaryComponents_SalaryComponentId",
                table: "PayrollEarnings",
                column: "SalaryComponentId",
                principalTable: "SalaryComponents",
                principalColumn: "Id");
        }
    }
}
