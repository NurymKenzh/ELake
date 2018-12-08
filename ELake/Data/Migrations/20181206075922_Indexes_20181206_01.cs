using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Indexes_20181206_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WaterLevel_LakeId",
                table: "WaterLevel",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transition_LakeId",
                table: "Transition",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_ToxicologicalIndicator_LakeId",
                table: "ToxicologicalIndicator",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasonalit_LakeId",
                table: "Seasonalit",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_IonsaltWaterComposition_LakeId",
                table: "IonsaltWaterComposition",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralHydrochemicalIndicator_LakeId",
                table: "GeneralHydrochemicalIndicator",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicsLakeArea_LakeId",
                table: "DynamicsLakeArea",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_BathigraphicAndVolumetricCurveData_LakeId",
                table: "BathigraphicAndVolumetricCurveData",
                column: "LakeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaterLevel_LakeId",
                table: "WaterLevel");

            migrationBuilder.DropIndex(
                name: "IX_Transition_LakeId",
                table: "Transition");

            migrationBuilder.DropIndex(
                name: "IX_ToxicologicalIndicator_LakeId",
                table: "ToxicologicalIndicator");

            migrationBuilder.DropIndex(
                name: "IX_Seasonalit_LakeId",
                table: "Seasonalit");

            migrationBuilder.DropIndex(
                name: "IX_IonsaltWaterComposition_LakeId",
                table: "IonsaltWaterComposition");

            migrationBuilder.DropIndex(
                name: "IX_GeneralHydrochemicalIndicator_LakeId",
                table: "GeneralHydrochemicalIndicator");

            migrationBuilder.DropIndex(
                name: "IX_DynamicsLakeArea_LakeId",
                table: "DynamicsLakeArea");

            migrationBuilder.DropIndex(
                name: "IX_BathigraphicAndVolumetricCurveData_LakeId",
                table: "BathigraphicAndVolumetricCurveData");
        }
    }
}
