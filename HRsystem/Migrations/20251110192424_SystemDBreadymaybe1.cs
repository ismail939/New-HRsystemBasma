using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class SystemDBreadymaybe1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    TotalHours = table.Column<int>(type: "int", nullable: true),
                    LateMinutes = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    PenaltyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeBasmas_EmployeeId",
                table: "HREmployeeBasmas",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeOffDays_EmployeeId",
                table: "HREmployeeOffDays",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeePenalties_EmployeeId",
                table: "HREmployeePenalties",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HREmployeeBasmas");

            migrationBuilder.DropTable(
                name: "HREmployeeOffDays");

            migrationBuilder.DropTable(
                name: "HREmployeePenalties");
        }
    }
}
