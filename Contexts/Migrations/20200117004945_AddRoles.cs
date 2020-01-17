using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("08d79ae6-7eb5-4426-82ba-7c4a4ae9d84b"), "26cd3943-23ff-41f5-86ed-8b867cf233b4", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("08d79ae6-7ec1-478f-867c-a8170f075a27"), "31bbe05d-7b5a-4b3a-9255-ed262a6a02c7", "Moderator", "MODERATOR" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("08d79ae6-7ec3-42ce-8a94-00a56192c379"), "352e1584-d439-45dc-8015-9428b4e47c76", "Uploader", "UPLOADER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("08d79ae6-7eb5-4426-82ba-7c4a4ae9d84b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("08d79ae6-7ec1-478f-867c-a8170f075a27"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("08d79ae6-7ec3-42ce-8a94-00a56192c379"));
        }
    }
}
