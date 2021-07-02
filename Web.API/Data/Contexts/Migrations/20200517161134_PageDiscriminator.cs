
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class PageDiscriminator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "Discriminator",
                "Pages",
                "PageType");

            migrationBuilder.Sql("UPDATE Pages SET PageType = 'Comic' WHERE PageType = 'ComicPage'");
            migrationBuilder.Sql("UPDATE Pages SET PageType = 'Novel' WHERE PageType = 'NovelPage'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "PageType",
                "Pages",
                "Discriminator");
            
            migrationBuilder.Sql("UPDATE Pages SET Discriminator = 'ComicPage' WHERE PageType = 'Comic'");
            migrationBuilder.Sql("UPDATE Pages SET Discriminator = 'NovelPage' WHERE PageType = 'Novel'");
        }
    }
}
