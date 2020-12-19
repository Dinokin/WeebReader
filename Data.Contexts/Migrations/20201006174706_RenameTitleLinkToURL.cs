
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class RenameTitleLinkToURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "PreviousChaptersLink",
                "Titles",
                "PreviousChaptersUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "PreviousChaptersUrl",
                "Titles",
                "PreviousChaptersLink");
        }
    }
}
