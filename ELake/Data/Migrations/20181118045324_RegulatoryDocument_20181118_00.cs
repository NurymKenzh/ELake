using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ELake.Data.Migrations
{
    public partial class RegulatoryDocument_20181118_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegulatoryDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Archival = table.Column<bool>(nullable: false),
                    DeletingJustification = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    File = table.Column<string>(nullable: true),
                    ForceEntryDay = table.Column<int>(nullable: true),
                    ForceEntryMonth = table.Column<int>(nullable: true),
                    ForceEntryYear = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NewDocumentId = table.Column<int>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    PreviousDocumentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegulatoryDocument_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegulatoryDocument_DocumentTypeId",
                table: "RegulatoryDocument",
                column: "DocumentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegulatoryDocument");
        }
    }
}
