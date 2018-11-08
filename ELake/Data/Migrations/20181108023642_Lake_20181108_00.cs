using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Lake_20181108_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VHBEN",
                table: "Lake",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VHBKK",
                table: "Lake",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VHBRU",
                table: "Lake",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VHBEN",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "VHBKK",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "VHBRU",
                table: "Lake");
        }
    }
}
