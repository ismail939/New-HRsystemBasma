using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class departmentstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HRDepartmentId",
                table: "HREmployees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HRDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HRDepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRDepartments_HRDepartments_HRDepartmentId",
                        column: x => x.HRDepartmentId,
                        principalTable: "HRDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRDepartments_HREmployees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "HREmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HRDepartments_HREmployees_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "HREmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HREmployees_HRDepartmentId",
                table: "HREmployees",
                column: "HRDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HRDepartments_HRDepartmentId",
                table: "HRDepartments",
                column: "HRDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HRDepartments_ManagerId",
                table: "HRDepartments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_HRDepartments_ParentDepartmentId",
                table: "HRDepartments",
                column: "ParentDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_HREmployees_HRDepartments_HRDepartmentId",
                table: "HREmployees",
                column: "HRDepartmentId",
                principalTable: "HRDepartments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HREmployees_HRDepartments_HRDepartmentId",
                table: "HREmployees");

            migrationBuilder.DropTable(
                name: "HRDepartments");

            migrationBuilder.DropIndex(
                name: "IX_HREmployees_HRDepartmentId",
                table: "HREmployees");

            migrationBuilder.DropColumn(
                name: "HRDepartmentId",
                table: "HREmployees");
        }
    }
}
