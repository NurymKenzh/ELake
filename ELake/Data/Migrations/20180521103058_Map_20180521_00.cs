using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Map_20180521_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MapId",
                table: "Layer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Map",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LayersId = table.Column<int[]>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Layer_MapId",
                table: "Layer",
                column: "MapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Layer_Map_MapId",
                table: "Layer",
                column: "MapId",
                principalTable: "Map",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layer_Map_MapId",
                table: "Layer");

            migrationBuilder.DropTable(
                name: "Map");

            migrationBuilder.DropIndex(
                name: "IX_Layer_MapId",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "MapId",
                table: "Layer");
        }
    }
}
