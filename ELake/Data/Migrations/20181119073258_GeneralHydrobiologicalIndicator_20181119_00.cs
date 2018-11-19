using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class GeneralHydrobiologicalIndicator_20181119_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralHydrobiologicalIndicator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    BenthosBiomass = table.Column<decimal>(nullable: true),
                    CurrentCommercialFishProductivity = table.Column<decimal>(nullable: true),
                    CurrentUsage = table.Column<decimal>(nullable: true),
                    FishCatchLimit = table.Column<decimal>(nullable: true),
                    FishId = table.Column<int>(nullable: false),
                    LakeId = table.Column<int>(nullable: false),
                    PotentialFishProducts = table.Column<decimal>(nullable: true),
                    PotentialGrowingVolume = table.Column<decimal>(nullable: true),
                    RecommendedUse = table.Column<decimal>(nullable: true),
                    SpeciesWealthIndex = table.Column<decimal>(nullable: true),
                    SurveyYear = table.Column<int>(nullable: false),
                    ZooplanktonBiomass = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralHydrobiologicalIndicator", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralHydrobiologicalIndicator");
        }
    }
}
