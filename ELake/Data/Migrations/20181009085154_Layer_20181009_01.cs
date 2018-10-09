using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20181009_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionEN",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionKK",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRU",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEN",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "DescriptionKK",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "DescriptionRU",
                table: "Layer");
        }
    }
}
