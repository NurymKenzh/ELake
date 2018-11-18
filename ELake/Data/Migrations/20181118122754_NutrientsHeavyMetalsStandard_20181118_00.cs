using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class NutrientsHeavyMetalsStandard_20181118_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutrientsHeavyMetalsStandard",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HazardClassCd = table.Column<int>(nullable: false),
                    HazardClassCo = table.Column<int>(nullable: false),
                    HazardClassCu = table.Column<int>(nullable: false),
                    HazardClassMn = table.Column<int>(nullable: false),
                    HazardClassNH4 = table.Column<int>(nullable: false),
                    HazardClassNO2 = table.Column<int>(nullable: false),
                    HazardClassNO3 = table.Column<int>(nullable: false),
                    HazardClassNi = table.Column<int>(nullable: false),
                    HazardClassPPO4 = table.Column<int>(nullable: false),
                    HazardClassPb = table.Column<int>(nullable: false),
                    HazardClassZn = table.Column<int>(nullable: false),
                    MPCCd = table.Column<decimal>(nullable: false),
                    MPCCo = table.Column<decimal>(nullable: false),
                    MPCCu = table.Column<decimal>(nullable: false),
                    MPCMn = table.Column<decimal>(nullable: false),
                    MPCNH4 = table.Column<decimal>(nullable: false),
                    MPCNO2 = table.Column<decimal>(nullable: false),
                    MPCNO3 = table.Column<decimal>(nullable: false),
                    MPCNi = table.Column<decimal>(nullable: false),
                    MPCPPO4 = table.Column<decimal>(nullable: false),
                    MPCPb = table.Column<decimal>(nullable: false),
                    MPCZn = table.Column<decimal>(nullable: false),
                    RegulatoryDocumentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientsHeavyMetalsStandard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutrientsHeavyMetalsStandard_RegulatoryDocument_RegulatoryDocumentId",
                        column: x => x.RegulatoryDocumentId,
                        principalTable: "RegulatoryDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NutrientsHeavyMetalsStandard_RegulatoryDocumentId",
                table: "NutrientsHeavyMetalsStandard",
                column: "RegulatoryDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutrientsHeavyMetalsStandard");
        }
    }
}
