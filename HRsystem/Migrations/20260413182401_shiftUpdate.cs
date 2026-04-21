using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class shiftUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HREmployees_EmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_HRShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_EmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_HRShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "HREmployeeShift");

            migrationBuilder.DropColumn(
                name: "HRShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "HREmployeeShift");

            migrationBuilder.RenameColumn(
                name: "ShiftMode",
                table: "HREmployeeShift",
                newName: "HREmployeeId");

            migrationBuilder.RenameColumn(
                name: "RequiredHours",
                table: "HREmployeeShift",
                newName: "ShiftOptionId");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "HREmployeeShift",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ShiftOverride",
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
                    table.PrimaryKey("PK_ShiftOverride", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftOverride_HREmployeeShift_HREmployeeShiftId",
                        column: x => x.HREmployeeShiftId,
                        principalTable: "HREmployeeShift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftOverride_HRShiftOptions_ShiftOptionId",
                        column: x => x.ShiftOptionId,
                        principalTable: "HRShiftOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_EmployeeId_FromDate",
                table: "HREmployeeShift",
                columns: new[] { "EmployeeId", "FromDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HREmployeeId",
                table: "HREmployeeShift",
                column: "HREmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_ShiftOptionId",
                table: "HREmployeeShift",
                column: "ShiftOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftOverride_HREmployeeShiftId_DayOfWeek",
                table: "ShiftOverride",
                columns: new[] { "HREmployeeShiftId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftOverride_ShiftOptionId",
                table: "ShiftOverride",
                column: "ShiftOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HREmployees_HREmployeeId",
                table: "HREmployeeShift",
                column: "HREmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_ShiftOptionId",
                table: "HREmployeeShift",
                column: "ShiftOptionId",
                principalTable: "HRShiftOptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HREmployees_HREmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_ShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.DropTable(
                name: "ShiftOverride");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_EmployeeId_FromDate",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_HREmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_ShiftOptionId",
                table: "HREmployeeShift");

            migrationBuilder.RenameColumn(
                name: "ShiftOptionId",
                table: "HREmployeeShift",
                newName: "RequiredHours");

            migrationBuilder.RenameColumn(
                name: "HREmployeeId",
                table: "HREmployeeShift",
                newName: "ShiftMode");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "HREmployeeShift",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "HREmployeeShift",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HRShiftOptionId",
                table: "HREmployeeShift",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "HREmployeeShift",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_EmployeeId",
                table: "HREmployeeShift",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HRShiftOptionId",
                table: "HREmployeeShift",
                column: "HRShiftOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HREmployees_EmployeeId",
                table: "HREmployeeShift",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HRShiftOptions_HRShiftOptionId",
                table: "HREmployeeShift",
                column: "HRShiftOptionId",
                principalTable: "HRShiftOptions",
                principalColumn: "Id");
        }
    }
}
