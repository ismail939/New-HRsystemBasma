using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class foreingkeyissue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_EmployeeId_FromDate",
                table: "HREmployeeShift");

            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_HREmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "HREmployeeShift");

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HREmployeeId_FromDate",
                table: "HREmployeeShift",
                columns: new[] { "HREmployeeId", "FromDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HREmployeeShift_HREmployeeId_FromDate",
                table: "HREmployeeShift");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "HREmployeeShift",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_EmployeeId_FromDate",
                table: "HREmployeeShift",
                columns: new[] { "EmployeeId", "FromDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HREmployeeShift_HREmployeeId",
                table: "HREmployeeShift",
                column: "HREmployeeId");
        }
    }
}
