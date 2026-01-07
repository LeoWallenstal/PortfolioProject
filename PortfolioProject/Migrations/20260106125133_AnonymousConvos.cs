using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioProject.Migrations
{
    public partial class AnonymousConvos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnonymousDisplayName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnonymousDisplayName",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnonymousPasswordHash",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnonymousDisplayName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "AnonymousDisplayName",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "AnonymousPasswordHash",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Conversations");
        }
    }
}
