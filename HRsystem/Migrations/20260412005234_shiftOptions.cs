using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class shiftOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HRShiftOptionId",
                table: "HREmployeeShift",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HRShiftOptionId",
                table: "HREmployeeShift",
                column: "HRShiftOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_HRShiftOptionId",
                table: "HREmployeeShift",
                column: "HRShiftOptionId",
                principalTable: "HRShiftOptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_HRShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropTable(
                name: "HRShiftOptions");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_HRShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropColumn(
                name: "HRShiftOptionId",
                table: "HREmployeeShift");
        }
    }
}
