using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Transition_20181119_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EphemeralPermanent = table.Column<decimal>(nullable: false),
                    EphemeralSeasonal = table.Column<decimal>(nullable: false),
                    LakeId = table.Column<int>(nullable: false),
                    LostPermanent = table.Column<decimal>(nullable: false),
                    LostSeasonal = table.Column<decimal>(nullable: false),
                    NewPermanent = table.Column<decimal>(nullable: false),
                    NewSeasonal = table.Column<decimal>(nullable: false),
                    NoСhange = table.Column<decimal>(nullable: false),
                    Permanent = table.Column<decimal>(nullable: false),
                    PermanentToDeasonal = table.Column<decimal>(nullable: false),
                    Seasonal = table.Column<decimal>(nullable: false),
                    SeasonalToPermanent = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transition", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transition");
        }
    }
}
