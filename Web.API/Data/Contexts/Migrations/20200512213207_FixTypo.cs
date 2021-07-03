using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class FixTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PreviousChapterLink",
                "Titles");

            migrationBuilder.AddColumn<string>(
                "PreviousChaptersLink",
                "Titles",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PreviousChaptersLink",
                "Titles");

            migrationBuilder.AddColumn<string>(
                "PreviousChapterLink",
                "Titles",
                "varchar(500) CHARACTER SET utf8mb4",
                maxLength: 500,
                nullable: true);
        }
    }
}
