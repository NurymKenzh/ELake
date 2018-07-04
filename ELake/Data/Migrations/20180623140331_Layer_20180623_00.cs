using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180623_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "ColorsSurfaceFlow",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValuesSurfaceFlow",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorsSurfaceFlow",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValuesSurfaceFlow",
                table: "Layer");
        }
    }
}
