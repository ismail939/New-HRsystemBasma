using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class BasmaFlagModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyBasmaFlags",
                table: "DailyBasmaFlags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DailyBasmaFlags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyBasmaFlags",
                table: "DailyBasmaFlags",
                column: "Day");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyBasmaFlags",
                table: "DailyBasmaFlags");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DailyBasmaFlags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyBasmaFlags",
                table: "DailyBasmaFlags",
                column: "Id");
        }
    }
}
