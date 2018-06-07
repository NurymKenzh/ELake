using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Layer_20180521_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameEN",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameKK",
                table: "Layer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRU",
                table: "Layer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEN",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "NameKK",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "NameRU",
                table: "Layer");
        }
    }
}
