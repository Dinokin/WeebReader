
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class NoPageInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Content",
                "Chapters",
                "MEDIUMTEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                "IX_Pages_ChapterId1",
                "Pages",
                "ChapterId");

            migrationBuilder.AddForeignKey(
                "FK_Pages_Chapters_ChapterId1",
                "Pages",
                "ChapterId",
                "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Pages_Chapters_ChapterId1",
                "Pages");

            migrationBuilder.DropIndex(
                "IX_Pages_ChapterId1",
                "Pages");

            migrationBuilder.AlterColumn<string>(
                "Content",
                "Chapters",
                "LONGTEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "MEDIUMTEXT",
                oldNullable: true);
        }
    }
}
