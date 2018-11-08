using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class LakesArchiveData_20181108_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LakesArchiveData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ArchivalInfoSource = table.Column<string>(nullable: true),
                    LakeAbsoluteHeight = table.Column<decimal>(nullable: true),
                    LakeId = table.Column<int>(nullable: false),
                    LakeLength = table.Column<decimal>(nullable: true),
                    LakeMaxDepth = table.Column<decimal>(nullable: true),
                    LakeMirrorArea = table.Column<decimal>(nullable: true),
                    LakeShorelineLength = table.Column<decimal>(nullable: true),
                    LakeWaterMass = table.Column<decimal>(nullable: true),
                    LakeWidth = table.Column<decimal>(nullable: true),
                    SurveyYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LakesArchiveData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LakesArchiveData");
        }
    }
}
