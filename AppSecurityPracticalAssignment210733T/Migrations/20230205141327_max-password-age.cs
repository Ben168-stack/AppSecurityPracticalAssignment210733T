using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppSecurityPracticalAssignment210733T.Migrations
{
    /// <inheritdoc />
    public partial class maxpasswordage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "maxPasswordAge",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "maxPasswordAge",
                table: "AspNetUsers");
        }
    }
}
