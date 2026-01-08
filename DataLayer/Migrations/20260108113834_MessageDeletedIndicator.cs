using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioProject.Migrations
{
    public partial class MessageDeletedIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedByReceiver",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeletedByReceiver",
                table: "Messages");
        }
    }
}
