using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class uniqueNationalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "HREmployees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HREmployees_NationalId",
                table: "HREmployees",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HREmployees_NationalId",
                table: "HREmployees");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "HREmployees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
