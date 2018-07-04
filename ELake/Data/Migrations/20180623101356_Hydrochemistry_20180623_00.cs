using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Hydrochemistry_20180623_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hydrochemistry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CIWP = table.Column<decimal>(nullable: true),
                    Ca = table.Column<decimal>(nullable: true),
                    Cd = table.Column<decimal>(nullable: true),
                    Cl = table.Column<decimal>(nullable: true),
                    Co = table.Column<decimal>(nullable: true),
                    Cu = table.Column<decimal>(nullable: true),
                    DissOxygWater = table.Column<decimal>(nullable: true),
                    HCO = table.Column<decimal>(nullable: true),
                    LakeId = table.Column<int>(nullable: false),
                    Mg = table.Column<decimal>(nullable: true),
                    Mineralization = table.Column<decimal>(nullable: true),
                    Mn = table.Column<decimal>(nullable: true),
                    NH = table.Column<decimal>(nullable: true),
                    NO2 = table.Column<decimal>(nullable: true),
                    NO3 = table.Column<decimal>(nullable: true),
                    NaK = table.Column<decimal>(nullable: true),
                    Ni = table.Column<decimal>(nullable: true),
                    OrganicSubstances = table.Column<decimal>(nullable: true),
                    PPO = table.Column<decimal>(nullable: true),
                    Pb = table.Column<decimal>(nullable: true),
                    PercentOxygWater = table.Column<decimal>(nullable: true),
                    SO = table.Column<decimal>(nullable: true),
                    TotalHardness = table.Column<decimal>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Zn = table.Column<decimal>(nullable: true),
                    pH = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hydrochemistry", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hydrochemistry");
        }
    }
}
