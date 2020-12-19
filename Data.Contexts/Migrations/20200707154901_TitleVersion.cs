
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class TitleVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                "Version",
                "Titles",
                "BIGINT UNSIGNED",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Version",
                "Titles");
        }
    }
}
