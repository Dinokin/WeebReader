using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class MoveNovelContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Content",
                "Chapters");

            migrationBuilder.CreateTable(
                "NovelChapterContents",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>("MEDIUMTEXT", nullable: false),
                    ChapterId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelChapterContents", x => x.Id);
                    table.ForeignKey(
                        "FK_NovelChapterContents_Chapters_ChapterId",
                        x => x.ChapterId,
                        "Chapters",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_NovelChapterContents_ChapterId",
                "NovelChapterContents",
                "ChapterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "NovelChapterContents");

            migrationBuilder.AddColumn<string>(
                "Content",
                "Chapters",
                "MEDIUMTEXT",
                nullable: true);
        }
    }
}
