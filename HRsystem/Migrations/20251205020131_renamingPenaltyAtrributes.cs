using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class renamingPenaltyAtrributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "PenaltyType",
               table: "HREmployeePenalties",
               newName: "Decision");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "HREmployeePenalties",
                newName: "Reason"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decision",
                table: "HREmployeePenalties",
                newName: "PenaltyType");

            migrationBuilder.RenameColumn(
            name: "Reason",
            table: "HREmployeePenalties",
            newName: "Notes"
        );
        }
    }
}
