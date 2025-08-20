using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattalgaziSosyalYardim.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class MotherBirthDate_ToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MotherBirthDate",
                table: "Applications",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MotherBirthDate",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
