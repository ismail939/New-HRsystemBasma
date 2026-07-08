using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRsystem.Migrations
{
    /// <inheritdoc />
    public partial class offDayBalanceAllTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Off",
                table: "HROffDayBalances",
                newName: "Unpaid");

            migrationBuilder.RenameColumn(
                name: "CompensatoryOfNationalHoliday",
                table: "HROffDayBalances",
                newName: "Sick");

            migrationBuilder.AddColumn<int>(
                name: "Compensatory",
                table: "HROffDayBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Exam",
                table: "HROffDayBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hajj",
                table: "HROffDayBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoCalculated",
                table: "HROffDayBalances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "HROffDayBalances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Maternity",
                table: "HROffDayBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OfficialHoliday",
                table: "HROffDayBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Compensatory",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "Exam",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "Hajj",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "IsAutoCalculated",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "Maternity",
                table: "HROffDayBalances");

            migrationBuilder.DropColumn(
                name: "OfficialHoliday",
                table: "HROffDayBalances");

            migrationBuilder.RenameColumn(
                name: "Unpaid",
                table: "HROffDayBalances",
                newName: "Off");

            migrationBuilder.RenameColumn(
                name: "Sick",
                table: "HROffDayBalances",
                newName: "CompensatoryOfNationalHoliday");
        }
    }
}
