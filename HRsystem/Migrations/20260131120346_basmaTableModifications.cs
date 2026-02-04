using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class basmaTableModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "TotalHours",
                table: "HREmployeeBasmas",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EarlyLeaveMinutes",
                table: "HREmployeeBasmas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ok",
                table: "HREmployeeBasmas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarlyLeaveMinutes",
                table: "HREmployeeBasmas");

            migrationBuilder.DropColumn(
                name: "Ok",
                table: "HREmployeeBasmas");

            migrationBuilder.AlterColumn<int>(
                name: "TotalHours",
                table: "HREmployeeBasmas",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
