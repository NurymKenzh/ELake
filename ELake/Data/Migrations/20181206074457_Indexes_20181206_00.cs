using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class Indexes_20181206_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WaterBalance_LakeId",
                table: "WaterBalance",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_LakesGlobalData_LakeId",
                table: "LakesGlobalData",
                column: "LakeId");

            migrationBuilder.CreateIndex(
                name: "IX_LakesArchiveData_LakeId",
                table: "LakesArchiveData",
                column: "LakeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaterBalance_LakeId",
                table: "WaterBalance");

            migrationBuilder.DropIndex(
                name: "IX_LakesGlobalData_LakeId",
                table: "LakesGlobalData");

            migrationBuilder.DropIndex(
                name: "IX_LakesArchiveData_LakeId",
                table: "LakesArchiveData");
        }
    }
}
