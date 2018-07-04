using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180618_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinValues",
                table: "Layer",
                newName: "MinValuesWaterLevel");

            migrationBuilder.RenameColumn(
                name: "Colors",
                table: "Layer",
                newName: "ColorsWaterLevel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinValuesWaterLevel",
                table: "Layer",
                newName: "MinValues");

            migrationBuilder.RenameColumn(
                name: "ColorsWaterLevel",
                table: "Layer",
                newName: "Colors");
        }
    }
}
