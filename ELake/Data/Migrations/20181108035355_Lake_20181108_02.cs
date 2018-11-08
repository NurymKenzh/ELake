using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Lake_20181108_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LakeShorelineLength2015",
                table: "Lake",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Lake",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Lake",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LakeShorelineLength2015",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Lake");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Lake");
        }
    }
}
