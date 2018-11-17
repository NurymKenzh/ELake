using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class IonsaltWaterComposition_20181117_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IonsaltWaterComposition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CaMgEq = table.Column<decimal>(nullable: true),
                    ClMgEq = table.Column<decimal>(nullable: true),
                    HCOMgEq = table.Column<decimal>(nullable: true),
                    LakeId = table.Column<int>(nullable: false),
                    MgMgEq = table.Column<decimal>(nullable: true),
                    NaKMgEq = table.Column<decimal>(nullable: true),
                    SOMgEq = table.Column<decimal>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IonsaltWaterComposition", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IonsaltWaterComposition");
        }
    }
}
