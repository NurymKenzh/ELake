using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Lake_20181107_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Area",
                table: "Lake",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VHB",
                table: "Lake",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VHU",
                table: "Lake",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "VHB",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "VHU",
                table: "Lake");
        }
    }
}
