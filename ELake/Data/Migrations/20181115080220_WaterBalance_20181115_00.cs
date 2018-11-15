using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class WaterBalance_20181115_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaterBalance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Evaporation = table.Column<decimal>(nullable: false),
                    LakeId = table.Column<int>(nullable: false),
                    Precipitation = table.Column<decimal>(nullable: false),
                    SurfaceFlow = table.Column<decimal>(nullable: false),
                    SurfaceOutflow = table.Column<decimal>(nullable: false),
                    UndergroundFlow = table.Column<decimal>(nullable: false),
                    UndergroundOutflow = table.Column<decimal>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterBalance", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterBalance");
        }
    }
}
