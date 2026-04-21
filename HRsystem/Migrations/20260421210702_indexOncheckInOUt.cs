using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class indexOncheckInOUt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CheckInOuts_UserId_CheckTime",
                table: "CheckInOuts",
                columns: new[] { "UserId", "CheckTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CheckInOuts_UserId_CheckTime",
                table: "CheckInOuts");
        }
    }
}
