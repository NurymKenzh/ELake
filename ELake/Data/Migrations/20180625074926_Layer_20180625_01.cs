using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180625_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryMineralization",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryMineralization",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryMineralization",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryMineralization",
                table: "Layer");
        }
    }
}
