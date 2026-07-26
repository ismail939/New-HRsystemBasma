using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class fixPayrollDetailFloatToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No schema changes needed - SQL Server 'float' already maps to .NET 'double'.
            // The model was changed from float to double in C#, which was causing:
            // "Unable to cast object of type 'System.Double' to type 'System.Single'"
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}