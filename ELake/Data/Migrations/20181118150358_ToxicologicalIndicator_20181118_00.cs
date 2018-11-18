using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class ToxicologicalIndicator_20181118_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToxicologicalIndicator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Cd = table.Column<decimal>(nullable: true),
                    Co = table.Column<decimal>(nullable: true),
                    Cu = table.Column<decimal>(nullable: true),
                    LakeId = table.Column<int>(nullable: false),
                    Mn = table.Column<decimal>(nullable: true),
                    NH4 = table.Column<decimal>(nullable: true),
                    NO2 = table.Column<decimal>(nullable: true),
                    NO3 = table.Column<decimal>(nullable: true),
                    Ni = table.Column<decimal>(nullable: true),
                    PPO4 = table.Column<decimal>(nullable: true),
                    Pb = table.Column<decimal>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Zn = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToxicologicalIndicator", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToxicologicalIndicator");
        }
    }
}
