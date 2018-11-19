using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class FishKind_20181119_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FishKind",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FamilyId = table.Column<int>(nullable: false),
                    FamilyNameKK = table.Column<string>(nullable: true),
                    FamilyNameLA = table.Column<string>(nullable: true),
                    FamilyNameRU = table.Column<string>(nullable: true),
                    FishId = table.Column<int>(nullable: false),
                    FishNameKK = table.Column<string>(nullable: true),
                    FishNameLA = table.Column<string>(nullable: true),
                    FishNameRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishKind", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FishKind");
        }
    }
}
