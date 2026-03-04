using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
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
                name: "DailyBasmaFlags",
                columns: table => new
                {
                    Day = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Taken = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBasmaFlags", x => x.Day);
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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarriageStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    LateMinutes = table.Column<int>(type: "int", nullable: true),
                    EarlyLeaveMinutes = table.Column<int>(type: "int", nullable: true),
                    Ok = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                name: "HREmployeePenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PenaltyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "HREmployeeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    RequiredHours = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShiftMode = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LateToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    EarlyLeaveToleranceMinutes = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HREmployeeShift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HREmployeeShift_HREmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HREmployees",
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
                    Off = table.Column<int>(type: "int", nullable: false),
                    CompensatoryOfNationalHoliday = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "IX_HREmployeePenalties_EmployeeId",
                table: "HREmployeePenalties",
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
                name: "IX_HREmployeeShift_EmployeeId",
                table: "HREmployeeShift",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HROffDayBalances_EmployeeId",
                table: "HROffDayBalances",
                column: "EmployeeId");

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
                name: "DailyBasmaFlags");

            migrationBuilder.DropTable(
                name: "HRApplierFiles");

            migrationBuilder.DropTable(
                name: "HREmployeeBasmas");

            migrationBuilder.DropTable(
                name: "HREmployeeFiles");

            migrationBuilder.DropTable(
                name: "HREmployeeOffDays");

            migrationBuilder.DropTable(
                name: "HREmployeePenalties");

            migrationBuilder.DropTable(
                name: "HREmployeeRates");

            migrationBuilder.DropTable(
                name: "HREmployeeShift");

            migrationBuilder.DropTable(
                name: "HROffDayBalances");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "HRAppliers");

            migrationBuilder.DropTable(
                name: "HREmployees");

            migrationBuilder.DropTable(
                name: "HRDepartments");
        }
    }
}
