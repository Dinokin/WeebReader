
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class TitlePreviousChapters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "UserName",
                "Users",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "NormalizedUserName",
                "Users",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "ClaimValue",
                "UserClaims",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "ClaimType",
                "UserClaims",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                "PreviousChapterLink",
                "Titles",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Tags",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20) CHARACTER SET utf8mb4",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                "NormalizedName",
                "Roles",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Roles",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PreviousChapterLink",
                "Titles");

            migrationBuilder.AlterColumn<string>(
                "UserName",
                "Users",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "NormalizedUserName",
                "Users",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "ClaimValue",
                "UserClaims",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "ClaimType",
                "UserClaims",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Tags",
                "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                "NormalizedName",
                "Roles",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Roles",
                "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
