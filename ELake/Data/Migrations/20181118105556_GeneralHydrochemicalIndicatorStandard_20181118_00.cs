using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class GeneralHydrochemicalIndicatorStandard_20181118_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralHydrochemicalIndicatorStandard",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Indicator = table.Column<string>(nullable: true),
                    LowerLimit = table.Column<decimal>(nullable: true),
                    MeasurementUnitId = table.Column<int>(nullable: false),
                    RegulatoryDocumentId = table.Column<int>(nullable: false),
                    UpperLimit = table.Column<decimal>(nullable: true),
                    Value = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralHydrochemicalIndicatorStandard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralHydrochemicalIndicatorStandard_MeasurementUnit_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralHydrochemicalIndicatorStandard_RegulatoryDocument_RegulatoryDocumentId",
                        column: x => x.RegulatoryDocumentId,
                        principalTable: "RegulatoryDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralHydrochemicalIndicatorStandard_MeasurementUnitId",
                table: "GeneralHydrochemicalIndicatorStandard",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralHydrochemicalIndicatorStandard_RegulatoryDocumentId",
                table: "GeneralHydrochemicalIndicatorStandard",
                column: "RegulatoryDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralHydrochemicalIndicatorStandard");
        }
    }
}
