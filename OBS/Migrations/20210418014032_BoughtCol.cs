using Microsoft.EntityFrameworkCore.Migrations;

namespace OBS.Migrations
{
    public partial class BoughtCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bought",
                table: "UserBooks",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bought",
                table: "UserBooks");
        }
    }
}
