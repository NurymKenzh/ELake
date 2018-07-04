using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180618_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Colors",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValues",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MinValues",
                table: "Layer");
        }
    }
}
