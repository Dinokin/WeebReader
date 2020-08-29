using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public partial class AddNovelEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Content",
                "Chapters",
                "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                "IX_Chapters_TitleId1",
                "Chapters",
                "TitleId");

            migrationBuilder.AddForeignKey(
                "FK_Chapters_Titles_TitleId1",
                "Chapters",
                "TitleId",
                "Titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Chapters_Titles_TitleId1",
                "Chapters");

            migrationBuilder.DropIndex(
                "IX_Chapters_TitleId1",
                "Chapters");

            migrationBuilder.DropColumn(
                "Content",
                "Chapters");
        }
    }
}
