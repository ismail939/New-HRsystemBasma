using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class PayrollModuleEnterpriseDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing string statuses to enum integer values before column type change
            // Draft=0, Reviewed=1, Approved=2, Locked=3
            migrationBuilder.Sql(@"
                UPDATE Payrolls SET Status = '0' WHERE Status = 'Draft';
                UPDATE Payrolls SET Status = '1' WHERE Status = 'Reviewed';
                UPDATE Payrolls SET Status = '2' WHERE Status = 'Approved';
                UPDATE Payrolls SET Status = '3' WHERE Status = 'Locked';
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Payrolls",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedDate",
                table: "Payrolls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayrollPolicyId",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommissionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HourlyRateMethod = table.Column<int>(type: "int", nullable: false),
                    FixedHourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WeekdayMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeekendMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HolidayMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinOvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxOvertimeHoursPerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxOvertimeHoursPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimePolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    CalculationMethod = table.Column<int>(type: "int", nullable: false),
                    DefaultAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FormulaExpression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsInsurable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkingDaysPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalendarDaysPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkingHoursPerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailySalaryCalcMethod = table.Column<int>(type: "int", nullable: false),
                    DailySalaryFixedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OvertimeBase = table.Column<int>(type: "int", nullable: false),
                    OvertimeWeekdayMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeWeekendMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeHolidayMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateDeductionMethod = table.Column<int>(type: "int", nullable: false),
                    AbsenceDeductionMethod = table.Column<int>(type: "int", nullable: false),
                    LeaveEncashmentMethod = table.Column<int>(type: "int", nullable: false),
                    MaxDeductionPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDeductionPerYear = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDeductionDays = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDeductionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionPlanId = table.Column<int>(type: "int", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionTiers_CommissionPlans_CommissionPlanId",
                        column: x => x.CommissionPlanId,
                        principalTable: "CommissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CommissionPlanId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PayrollItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_CommissionPlans_CommissionPlanId",
                        column: x => x.CommissionPlanId,
                        principalTable: "CommissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCommissionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CommissionPlanId = table.Column<int>(type: "int", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCommissionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeCommissionPlans_CommissionPlans_CommissionPlanId",
                        column: x => x.CommissionPlanId,
                        principalTable: "CommissionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeCommissionPlans_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeOvertimePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OvertimePolicyId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeOvertimePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeOvertimePolicies_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeOvertimePolicies_OvertimePolicies_OvertimePolicyId",
                        column: x => x.OvertimePolicyId,
                        principalTable: "OvertimePolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OvertimePolicyId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OvertimeMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Multiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeType = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayrollItemId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeEntries_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OvertimeEntries_OvertimePolicies_OvertimePolicyId",
                        column: x => x.OvertimePolicyId,
                        principalTable: "OvertimePolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePayrollComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollComponentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePayrollComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollComponents_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollComponents_PayrollComponents_PayrollComponentId",
                        column: x => x.PayrollComponentId,
                        principalTable: "PayrollComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollComponentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollComponentId = table.Column<int>(type: "int", nullable: false),
                    OldAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollComponentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollComponentHistories_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollComponentHistories_PayrollComponents_PayrollComponentId",
                        column: x => x.PayrollComponentId,
                        principalTable: "PayrollComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollComponentHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PayrollItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollComponentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsManual = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollItems_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollItems_PayrollComponents_PayrollComponentId",
                        column: x => x.PayrollComponentId,
                        principalTable: "PayrollComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollItems_Payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeductionLimitPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: true),
                    MaxDeductionPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDeductionPerYear = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxMoneyPerPenalty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxDaysPerPenalty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxPercentageOfSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionLimitPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeductionLimitPolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentPayrollPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentPayrollPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentPayrollPolicies_HRDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "HRDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentPayrollPolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePayrollPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePayrollPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollPolicies_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollPolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_Month_Year",
                table: "Payrolls",
                columns: new[] { "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_PayrollPolicyId",
                table: "Payrolls",
                column: "PayrollPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTiers_CommissionPlanId",
                table: "CommissionTiers",
                column: "CommissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_CommissionPlanId",
                table: "CommissionTransactions",
                column: "CommissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_EmployeeId_TransactionDate",
                table: "CommissionTransactions",
                columns: new[] { "EmployeeId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DeductionLimitPolicies_PayrollPolicyId",
                table: "DeductionLimitPolicies",
                column: "PayrollPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPayrollPolicies_DepartmentId",
                table: "DepartmentPayrollPolicies",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPayrollPolicies_PayrollPolicyId",
                table: "DepartmentPayrollPolicies",
                column: "PayrollPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCommissionPlans_CommissionPlanId",
                table: "EmployeeCommissionPlans",
                column: "CommissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCommissionPlans_EmployeeId",
                table: "EmployeeCommissionPlans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimePolicies_EmployeeId",
                table: "EmployeeOvertimePolicies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimePolicies_OvertimePolicyId",
                table: "EmployeeOvertimePolicies",
                column: "OvertimePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollComponents_EmployeeId_PayrollComponentId_StartDate",
                table: "EmployeePayrollComponents",
                columns: new[] { "EmployeeId", "PayrollComponentId", "StartDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollComponents_PayrollComponentId",
                table: "EmployeePayrollComponents",
                column: "PayrollComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollPolicies_EmployeeId",
                table: "EmployeePayrollPolicies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollPolicies_PayrollPolicyId",
                table: "EmployeePayrollPolicies",
                column: "PayrollPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeEntries_EmployeeId_Date",
                table: "OvertimeEntries",
                columns: new[] { "EmployeeId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeEntries_OvertimePolicyId",
                table: "OvertimeEntries",
                column: "OvertimePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComponentHistories_ChangedByUserId",
                table: "PayrollComponentHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComponentHistories_EmployeeId",
                table: "PayrollComponentHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComponentHistories_PayrollComponentId",
                table: "PayrollComponentHistories",
                column: "PayrollComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComponents_Code",
                table: "PayrollComponents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_EmployeeId",
                table: "PayrollItems",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_PayrollComponentId",
                table: "PayrollItems",
                column: "PayrollComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_PayrollId_EmployeeId",
                table: "PayrollItems",
                columns: new[] { "PayrollId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_SourceType_SourceId",
                table: "PayrollItems",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_PayrollPolicies_PayrollPolicyId",
                table: "Payrolls",
                column: "PayrollPolicyId",
                principalTable: "PayrollPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_PayrollPolicies_PayrollPolicyId",
                table: "Payrolls");

            migrationBuilder.DropTable(
                name: "CommissionTiers");

            migrationBuilder.DropTable(
                name: "CommissionTransactions");

            migrationBuilder.DropTable(
                name: "DeductionLimitPolicies");

            migrationBuilder.DropTable(
                name: "DepartmentPayrollPolicies");

            migrationBuilder.DropTable(
                name: "EmployeeCommissionPlans");

            migrationBuilder.DropTable(
                name: "EmployeeOvertimePolicies");

            migrationBuilder.DropTable(
                name: "EmployeePayrollComponents");

            migrationBuilder.DropTable(
                name: "EmployeePayrollPolicies");

            migrationBuilder.DropTable(
                name: "OvertimeEntries");

            migrationBuilder.DropTable(
                name: "PayrollComponentHistories");

            migrationBuilder.DropTable(
                name: "PayrollItems");

            migrationBuilder.DropTable(
                name: "CommissionPlans");

            migrationBuilder.DropTable(
                name: "PayrollPolicies");

            migrationBuilder.DropTable(
                name: "OvertimePolicies");

            migrationBuilder.DropTable(
                name: "PayrollComponents");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_Month_Year",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_PayrollPolicyId",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "LockedDate",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "PayrollPolicyId",
                table: "Payrolls");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payrolls",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
