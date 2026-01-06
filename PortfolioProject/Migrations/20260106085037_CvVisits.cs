using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioProject.Migrations
{
    public partial class CvVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CvVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvVisits_AspNetUsers_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CvVisits_Cvs_CvId",
                        column: x => x.CvId,
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvVisits_CvId_VisitorId",
                table: "CvVisits",
                columns: new[] { "CvId", "VisitorId" },
                unique: true,
                filter: "[VisitorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CvVisits_VisitorId",
                table: "CvVisits",
                column: "VisitorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvVisits");
        }
    }
}
