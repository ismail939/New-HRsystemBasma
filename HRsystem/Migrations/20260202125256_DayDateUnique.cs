using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class DayDateUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropIndex(
                name: "IX_HREmployeeBasmas_EmployeeId",
                table: "HREmployeeBasmas");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeBasmas_EmployeeId_DayDate",
                table: "HREmployeeBasmas",
                columns: new[] { "EmployeeId", "DayDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HREmployeeBasmas_EmployeeId_DayDate",
                table: "HREmployeeBasmas");


            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeBasmas_EmployeeId",
                table: "HREmployeeBasmas",
                column: "EmployeeId");
        }
    }
}
