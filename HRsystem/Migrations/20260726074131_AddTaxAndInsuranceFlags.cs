using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxAndInsuranceFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInsurable",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                table: "SalaryComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInsurable",
                table: "PayrollEarnings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                table: "PayrollEarnings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InsurableAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxableAmount",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsInsurable",
                table: "PayrollDeductions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                table: "PayrollDeductions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "IsInsurable",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "IsTaxable",
                table: "SalaryComponents");

            migrationBuilder.DropColumn(
                name: "IsInsurable",
                table: "PayrollEarnings");

            migrationBuilder.DropColumn(
                name: "IsTaxable",
                table: "PayrollEarnings");

            migrationBuilder.DropColumn(
                name: "InsurableAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "InsuranceAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "TaxableAmount",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "IsInsurable",
                table: "PayrollDeductions");

            migrationBuilder.DropColumn(
                name: "IsTaxable",
                table: "PayrollDeductions");
        }
    }
}
