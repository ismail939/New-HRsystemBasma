using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class modifying : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_EmployeeId",
                table: "HREmployeeShift",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeRates_EmployeeId",
                table: "HREmployeeRates",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeRates_HREmployees_EmployeeId",
                table: "HREmployeeRates",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployeeShift_HREmployees_EmployeeId",
                table: "HREmployeeShift",
                column: "EmployeeId",
                principalTable: "HREmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeRates_HREmployees_EmployeeId",
                table: "HREmployeeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_HREmployeeShift_HREmployees_EmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_EmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeRates_EmployeeId",
                table: "HREmployeeRates");
        }
    }
}
