using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Seasonalit_20181119_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seasonalit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    April = table.Column<decimal>(nullable: false),
                    August = table.Column<decimal>(nullable: false),
                    December = table.Column<decimal>(nullable: false),
                    February = table.Column<decimal>(nullable: false),
                    January = table.Column<decimal>(nullable: false),
                    July = table.Column<decimal>(nullable: false),
                    June = table.Column<decimal>(nullable: false),
                    LakeId = table.Column<int>(nullable: false),
                    March = table.Column<decimal>(nullable: false),
                    May = table.Column<decimal>(nullable: false),
                    NoData = table.Column<decimal>(nullable: false),
                    November = table.Column<decimal>(nullable: false),
                    October = table.Column<decimal>(nullable: false),
                    September = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasonalit", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seasonalit");
        }
    }
}
