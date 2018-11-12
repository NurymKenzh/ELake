using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class LakesGlobalData_20181112_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LakesGlobalData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Continent_ENG = table.Column<string>(nullable: true),
                    Continent_KZ = table.Column<string>(nullable: true),
                    Continent_RU = table.Column<string>(nullable: true),
                    Country_ENG = table.Column<string>(nullable: true),
                    Country_KZ = table.Column<string>(nullable: true),
                    Country_RU = table.Column<string>(nullable: true),
                    Depth_avg = table.Column<decimal>(nullable: true),
                    Dis_avg = table.Column<decimal>(nullable: true),
                    Elevation = table.Column<decimal>(nullable: true),
                    Hylak_id = table.Column<int>(nullable: false),
                    LakeId = table.Column<int>(nullable: false),
                    Lake_area = table.Column<decimal>(nullable: true),
                    Lake_name_ENG = table.Column<string>(nullable: true),
                    Lake_name_KZ = table.Column<string>(nullable: true),
                    Lake_name_RU = table.Column<string>(nullable: true),
                    Pour_lat = table.Column<decimal>(nullable: true),
                    Pour_long = table.Column<decimal>(nullable: true),
                    Res_time = table.Column<decimal>(nullable: true),
                    Shore_dev = table.Column<decimal>(nullable: true),
                    Shore_len = table.Column<decimal>(nullable: true),
                    Slope_100 = table.Column<decimal>(nullable: true),
                    Vol_total = table.Column<decimal>(nullable: true),
                    Wshd_area = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LakesGlobalData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LakesGlobalData");
        }
    }
}
