using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class shiftOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftOverride_HREmployeeShift_HREmployeeShiftId",
                table: "ShiftOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftOverride_HRShiftOptions_ShiftOptionId",
                table: "ShiftOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftOverride",
                table: "ShiftOverride");

            migrationBuilder.RenameTable(
                name: "ShiftOverride",
                newName: "ShiftOverrides");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftOverride_ShiftOptionId",
                table: "ShiftOverrides",
                newName: "IX_ShiftOverrides_ShiftOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftOverride_HREmployeeShiftId_DayOfWeek",
                table: "ShiftOverrides",
                newName: "IX_ShiftOverrides_HREmployeeShiftId_DayOfWeek");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftOverrides",
                table: "ShiftOverrides",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftOverrides_HREmployeeShift_HREmployeeShiftId",
                table: "ShiftOverrides",
                column: "HREmployeeShiftId",
                principalTable: "HREmployeeShift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftOverrides_HRShiftOptions_ShiftOptionId",
                table: "ShiftOverrides",
                column: "ShiftOptionId",
                principalTable: "HRShiftOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftOverrides_HREmployeeShift_HREmployeeShiftId",
                table: "ShiftOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftOverrides_HRShiftOptions_ShiftOptionId",
                table: "ShiftOverrides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftOverrides",
                table: "ShiftOverrides");

            migrationBuilder.RenameTable(
                name: "ShiftOverrides",
                newName: "ShiftOverride");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftOverrides_ShiftOptionId",
                table: "ShiftOverride",
                newName: "IX_ShiftOverride_ShiftOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ShiftOverrides_HREmployeeShiftId_DayOfWeek",
                table: "ShiftOverride",
                newName: "IX_ShiftOverride_HREmployeeShiftId_DayOfWeek");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftOverride",
                table: "ShiftOverride",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftOverride_HREmployeeShift_HREmployeeShiftId",
                table: "ShiftOverride",
                column: "HREmployeeShiftId",
                principalTable: "HREmployeeShift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftOverride_HRShiftOptions_ShiftOptionId",
                table: "ShiftOverride",
                column: "ShiftOptionId",
                principalTable: "HRShiftOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
