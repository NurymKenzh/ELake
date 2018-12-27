using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class DynamicsLakeArea_20181227_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoWater",
                table: "DynamicsLakeArea",
                newName: "NotWater");

            migrationBuilder.RenameColumn(
                name: "NoData",
                table: "DynamicsLakeArea",
                newName: "NoDataPers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotWater",
                table: "DynamicsLakeArea",
                newName: "NoWater");

            migrationBuilder.RenameColumn(
                name: "NoDataPers",
                table: "DynamicsLakeArea",
                newName: "NoData");
        }
    }
}
