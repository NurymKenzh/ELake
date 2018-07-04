using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180626_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCIWP",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCa",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCd",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCl",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCo",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryCu",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryDissOxygWater",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryHCO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryMg",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryMn",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryNH",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryNO2",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryNO3",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryNaK",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryNi",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryOrganicSubstances",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryPPO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryPb",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryPercentOxygWater",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistrySO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryTotalHardness",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistryZn",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsHydrochemistrypH",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCIWP",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCa",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCd",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCl",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCo",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryCu",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryDissOxygWater",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryHCO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryMg",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryMn",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryNH",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryNO2",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryNO3",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryNaK",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryNi",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryOrganicSubstances",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryPPO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryPb",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryPercentOxygWater",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistrySO",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryTotalHardness",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistryZn",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesHydrochemistrypH",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCIWP",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCa",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCd",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCl",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCo",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryCu",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryDissOxygWater",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryHCO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryMg",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryMn",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryNH",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryNO2",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryNO3",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryNaK",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryNi",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryOrganicSubstances",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryPPO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryPb",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryPercentOxygWater",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistrySO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryTotalHardness",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistryZn",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsHydrochemistrypH",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCIWP",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCa",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCd",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCl",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCo",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryCu",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryDissOxygWater",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryHCO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryMg",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryMn",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryNH",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryNO2",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryNO3",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryNaK",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryNi",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryOrganicSubstances",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryPPO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryPb",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryPercentOxygWater",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistrySO",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryTotalHardness",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistryZn",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesHydrochemistrypH",
                table: "Layer");
        }
    }
}
