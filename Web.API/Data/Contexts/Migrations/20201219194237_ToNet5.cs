using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class ToNet5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Chapters_Titles_TitleId1",
                "Chapters");

            migrationBuilder.DropForeignKey(
                "FK_Pages_Chapters_ChapterId1",
                "Pages");

            migrationBuilder.DropTable(
                "NovelChapterContents");

            migrationBuilder.DropIndex(
                "IX_Pages_ChapterId1",
                "Pages");

            migrationBuilder.DropIndex(
                "IX_Chapters_TitleId1",
                "Chapters");

            migrationBuilder.AddColumn<string>(
                "Content",
                "Chapters",
                "MEDIUMTEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Content",
                "Chapters");

            migrationBuilder.CreateTable(
                "NovelChapterContents",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false),
                    ChapterId = table.Column<Guid>("char(36)", nullable: false),
                    Content = table.Column<string>("MEDIUMTEXT", nullable: false)
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
                "IX_Pages_ChapterId1",
                "Pages",
                "ChapterId");

            migrationBuilder.CreateIndex(
                "IX_Chapters_TitleId1",
                "Chapters",
                "TitleId");

            migrationBuilder.CreateIndex(
                "IX_NovelChapterContents_ChapterId",
                "NovelChapterContents",
                "ChapterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                "FK_Chapters_Titles_TitleId1",
                "Chapters",
                "TitleId",
                "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Pages_Chapters_ChapterId1",
                "Pages",
                "ChapterId",
                "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
