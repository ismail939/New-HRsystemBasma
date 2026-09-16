using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxBracketsAndInsurancePolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumInsurableSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaximumInsurableSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                    table.ForeignKey("FK_InsurancePolicies_PayrollPolicies_PayrollPolicyId", x => x.PayrollPolicyId, "PayrollPolicies", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    PayrollPolicyId = table.Column<int>(type: "int", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FixedDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBrackets", x => x.Id);
                    table.ForeignKey("FK_TaxBrackets_PayrollPolicies_PayrollPolicyId", x => x.PayrollPolicyId, "PayrollPolicies", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_InsurancePolicies_PayrollPolicyId", "InsurancePolicies", "PayrollPolicyId");
            migrationBuilder.CreateIndex("IX_TaxBrackets_PayrollPolicyId", "TaxBrackets", "PayrollPolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "InsurancePolicies");
            migrationBuilder.DropTable(name: "TaxBrackets");
        }
    }
}
