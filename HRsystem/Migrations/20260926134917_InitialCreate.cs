using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckInOuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInOuts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRAppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRAppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRShiftOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftMode = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredHours = table.Column<int>(type: "int", nullable: true),
                    LateToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    EarlyLeaveToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRShiftOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HourlyRateMethod = table.Column<int>(type: "int", nullable: false),
                    FixedHourlyRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WeekdayMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WeekendMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    HolidayMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MinOvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxOvertimeHoursPerDay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxOvertimeHoursPerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                    DefaultAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DefaultPercentage = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    WorkingDaysPerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CalendarDaysPerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WorkingHoursPerDay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailySalaryCalcMethod = table.Column<int>(type: "int", nullable: false),
                    DailySalaryFixedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OvertimeBase = table.Column<int>(type: "int", nullable: false),
                    OvertimeWeekdayMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OvertimeWeekendMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OvertimeHolidayMultiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LateDeductionMethod = table.Column<int>(type: "int", nullable: false),
                    AbsenceDeductionMethod = table.Column<int>(type: "int", nullable: false),
                    LeaveEncashmentMethod = table.Column<int>(type: "int", nullable: false),
                    MaxDeductionPerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDeductionPerYear = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDeductionDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDeductionPercentage = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPolicies", x => x.Id);
                });

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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionPlanId = table.Column<int>(type: "int", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
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
                name: "HRApplierFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplierId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRApplierFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRApplierFiles_HRAppliers_ApplierId",
                        column: x => x.ApplierId,
                        principalTable: "HRAppliers",
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
                    MaxDeductionPerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDeductionPerYear = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxMoneyPerPenalty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDaysPerPenalty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxPercentageOfSalary = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EmployerRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MinimumInsurableSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumInsurableSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payrolls_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FixedDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBrackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxBrackets_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeductionPercentage = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    FromValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    DeductionUnit = table.Column<int>(type: "int", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeductionPercentage = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                name: "CommissionTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CommissionPlanId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                        name: "FK_DepartmentPayrollPolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
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
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                        name: "FK_EmployeeOvertimePolicies_OvertimePolicies_OvertimePolicyId",
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
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                        name: "FK_EmployeePayrollComponents_PayrollComponents_PayrollComponentId",
                        column: x => x.PayrollComponentId,
                        principalTable: "PayrollComponents",
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
                        name: "FK_EmployeePayrollPolicies_PayrollPolicies_PayrollPolicyId",
                        column: x => x.PayrollPolicyId,
                        principalTable: "PayrollPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    DeductionValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeductionDays = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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

            migrationBuilder.CreateTable(
                name: "HRDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRDepartments_HRDepartments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "HRDepartments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HREmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarriageStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasmaId = table.Column<int>(type: "int", nullable: true),
                    HRDepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployees_HRDepartments_HRDepartmentId",
                        column: x => x.HRDepartmentId,
                        principalTable: "HRDepartments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HREmployeeBasmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalHours = table.Column<float>(type: "real", nullable: true),
                    LateMinutes = table.Column<float>(type: "real", nullable: true),
                    EarlyLeaveMinutes = table.Column<float>(type: "real", nullable: true),
                    OvertimeMinutes = table.Column<float>(type: "real", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OffDayType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeBasmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeBasmas_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HREmployeeFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeFiles_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HREmployeeOffDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OffDayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OffDayType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeOffDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeOffDays_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HREmployeeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeRates_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HREmployeeShift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LateToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    EarlyLeaveToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    ShiftOptionId = table.Column<int>(type: "int", nullable: true),
                    HREmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeShift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeShift_HREmployees_HREmployeeId",
                        column: x => x.HREmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HREmployeeShift_HRShiftOptions_ShiftOptionId",
                        column: x => x.ShiftOptionId,
                        principalTable: "HRShiftOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HROffDayBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Annual = table.Column<int>(type: "int", nullable: false),
                    Casual = table.Column<int>(type: "int", nullable: false),
                    Sick = table.Column<int>(type: "int", nullable: false),
                    Hajj = table.Column<int>(type: "int", nullable: false),
                    Maternity = table.Column<int>(type: "int", nullable: false),
                    Unpaid = table.Column<int>(type: "int", nullable: false),
                    Compensatory = table.Column<int>(type: "int", nullable: false),
                    OfficialHoliday = table.Column<int>(type: "int", nullable: false),
                    Exam = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAutoCalculated = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HROffDayBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HROffDayBalances_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id");
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
                    OvertimeMinutes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Multiplier = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "PayrollComponentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollComponentId = table.Column<int>(type: "int", nullable: false),
                    OldAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NewAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "PayrollDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InsurableAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InsuranceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PresentDays = table.Column<int>(type: "int", nullable: false),
                    AbsentDays = table.Column<int>(type: "int", nullable: false),
                    LateMinutes = table.Column<double>(type: "float", nullable: false),
                    OvertimeHours = table.Column<double>(type: "float", nullable: false),
                    PaidLeaves = table.Column<int>(type: "int", nullable: false),
                    UnpaidLeaves = table.Column<int>(type: "int", nullable: false),
                    OfficialHolidays = table.Column<int>(type: "int", nullable: false),
                    DailySalaryRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollDetails_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollDetails_Payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Requests_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Users_RespondedByUserId",
                        column: x => x.RespondedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    ShiftOptionId = table.Column<int>(type: "int", nullable: false),
                    HREmployeeShiftId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftOverrides_HREmployeeShift_HREmployeeShiftId",
                        column: x => x.HREmployeeShiftId,
                        principalTable: "HREmployeeShift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftOverrides_HRShiftOptions_ShiftOptionId",
                        column: x => x.ShiftOptionId,
                        principalTable: "HRShiftOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollDetailId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsInsurable = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollDeductions_PayrollDetails_PayrollDetailId",
                        column: x => x.PayrollDetailId,
                        principalTable: "PayrollDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollEarnings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollDetailId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsInsurable = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollEarnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollEarnings_PayrollDetails_PayrollDetailId",
                        column: x => x.PayrollDetailId,
                        principalTable: "PayrollDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Requests_RelatedRequestId",
                        column: x => x.RelatedRequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckInOuts_UserId_CheckTime",
                table: "CheckInOuts",
                columns: new[] { "UserId", "CheckTime" });

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
                name: "IX_HRApplierFiles_ApplierId",
                table: "HRApplierFiles",
                column: "ApplierId");

            migrationBuilder.CreateIndex(
                name: "IX_HRDepartments_ManagerId",
                table: "HRDepartments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_HRDepartments_ParentDepartmentId",
                table: "HRDepartments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeBasmas_EmployeeId_DayDate",
                table: "HREmployeeBasmas",
                columns: new[] { "EmployeeId", "DayDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeFiles_EmployeeId",
                table: "HREmployeeFiles",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeOffDays_EmployeeId",
                table: "HREmployeeOffDays",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeRates_EmployeeId",
                table: "HREmployeeRates",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployees_HRDepartmentId",
                table: "HREmployees",
                column: "HRDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployees_NationalId",
                table: "HREmployees",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HREmployeeId_FromDate",
                table: "HREmployeeShift",
                columns: new[] { "HREmployeeId", "FromDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_ShiftOptionId",
                table: "HREmployeeShift",
                column: "ShiftOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_HROffDayBalances_EmployeeId",
                table: "HROffDayBalances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_PayrollPolicyId",
                table: "InsurancePolicies",
                column: "PayrollPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedRequestId",
                table: "Notifications",
                column: "RelatedRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

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
                name: "IX_PayrollDeductions_PayrollDetailId",
                table: "PayrollDeductions",
                column: "PayrollDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_EmployeeId",
                table: "PayrollDetails",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollDetails_PayrollId",
                table: "PayrollDetails",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEarnings_PayrollDetailId",
                table: "PayrollEarnings",
                column: "PayrollDetailId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreatedAt",
                table: "Requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreatedByUserId",
                table: "Requests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EmployeeId",
                table: "Requests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RespondedByUserId",
                table: "Requests",
                column: "RespondedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Status",
                table: "Requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftOverrides_HREmployeeShiftId_DayOfWeek",
                table: "ShiftOverrides",
                columns: new[] { "HREmployeeShiftId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftOverrides_ShiftOptionId",
                table: "ShiftOverrides",
                column: "ShiftOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxBrackets_PayrollPolicyId",
                table: "TaxBrackets",
                column: "PayrollPolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionTransactions_HREmployees_EmployeeId",
                table: "CommissionTransactions",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPayrollPolicies_HRDepartments_DepartmentId",
                table: "DepartmentPayrollPolicies",
                column: "DepartmentId",
                principalTable: "HRDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCommissionPlans_HREmployees_EmployeeId",
                table: "EmployeeCommissionPlans",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimePolicies_HREmployees_EmployeeId",
                table: "EmployeeOvertimePolicies",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePayrollComponents_HREmployees_EmployeeId",
                table: "EmployeePayrollComponents",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePayrollPolicies_HREmployees_EmployeeId",
                table: "EmployeePayrollPolicies",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePenalties_HREmployees_EmployeeId",
                table: "EmployeePenalties",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HRDepartments_HREmployees_ManagerId",
                table: "HRDepartments",
                column: "ManagerId",
                principalTable: "HREmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HRDepartments_HREmployees_ManagerId",
                table: "HRDepartments");

            migrationBuilder.DropTable(
                name: "CheckInOuts");

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
                name: "EmployeePenalties");

            migrationBuilder.DropTable(
                name: "HRApplierFiles");

            migrationBuilder.DropTable(
                name: "HREmployeeBasmas");

            migrationBuilder.DropTable(
                name: "HREmployeeFiles");

            migrationBuilder.DropTable(
                name: "HREmployeeOffDays");

            migrationBuilder.DropTable(
                name: "HREmployeeRates");

            migrationBuilder.DropTable(
                name: "HRLogs");

            migrationBuilder.DropTable(
                name: "HROffDayBalances");

            migrationBuilder.DropTable(
                name: "InsurancePolicies");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OvertimeEntries");

            migrationBuilder.DropTable(
                name: "PayrollComponentHistories");

            migrationBuilder.DropTable(
                name: "PayrollDeductions");

            migrationBuilder.DropTable(
                name: "PayrollEarnings");

            migrationBuilder.DropTable(
                name: "PayrollItems");

            migrationBuilder.DropTable(
                name: "ShiftOverrides");

            migrationBuilder.DropTable(
                name: "TaxBrackets");

            migrationBuilder.DropTable(
                name: "CommissionPlans");

            migrationBuilder.DropTable(
                name: "PenaltyEscalations");

            migrationBuilder.DropTable(
                name: "PenaltyLevels");

            migrationBuilder.DropTable(
                name: "HRAppliers");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "OvertimePolicies");

            migrationBuilder.DropTable(
                name: "PayrollDetails");

            migrationBuilder.DropTable(
                name: "PayrollComponents");

            migrationBuilder.DropTable(
                name: "HREmployeeShift");

            migrationBuilder.DropTable(
                name: "PenaltyRules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropTable(
                name: "HRShiftOptions");

            migrationBuilder.DropTable(
                name: "PayrollPolicies");

            migrationBuilder.DropTable(
                name: "HREmployees");

            migrationBuilder.DropTable(
                name: "HRDepartments");
        }
    }
}
