using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180625_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ColorsEvaporation",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsPrecipitation",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsSurfaceOutflow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsUndergroundFlow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ColorsUndergroundOutflow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesEvaporation",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesPrecipitation",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesSurfaceOutflow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesUndergroundFlow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesUndergroundOutflow",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorsEvaporation",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsPrecipitation",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsSurfaceOutflow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsUndergroundFlow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "ColorsUndergroundOutflow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesEvaporation",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesPrecipitation",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesSurfaceOutflow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesUndergroundFlow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesUndergroundOutflow",
                table: "Layer");
        }
    }
}
