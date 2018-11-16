using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class GeneralHydrochemicalIndicator_20181116_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralHydrochemicalIndicator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DissOxygWater = table.Column<decimal>(nullable: true),
                    LakeId = table.Column<int>(nullable: false),
                    Mineralization = table.Column<decimal>(nullable: true),
                    OrganicSubstances = table.Column<decimal>(nullable: true),
                    PercentOxygWater = table.Column<decimal>(nullable: true),
                    TotalHardness = table.Column<decimal>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    pH = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralHydrochemicalIndicator", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralHydrochemicalIndicator");
        }
    }
}
