using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class LakeKATO_20180912_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LakeKATO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    KATOId = table.Column<int>(nullable: false),
                    LakeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LakeKATO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LakeKATO_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LakeKATO_KATOId",
                table: "LakeKATO",
                column: "KATOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LakeKATO");
        }
    }
}
