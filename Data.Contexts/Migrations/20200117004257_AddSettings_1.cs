using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class AddSettings_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Key", "Value" },
                values: new object[,]
                {
                    { new Guid("040569bc-3251-47d1-b51a-1a728c3d49ec"), "SiteName", "WeebReader" },
                    { new Guid("a49f13c1-bd9a-41ac-90a4-4d9051b0cdec"), "SiteDescription", "We read weebs." },
                    { new Guid("94010814-1ba1-4fca-8e57-a879ef51ba1a"), "SiteAddress", "http://127.0.0.1:5000" },
                    { new Guid("1c629bb9-e897-4e6c-9017-300aef64d077"), "SiteEmail", "" },
                    { new Guid("af86f645-4209-4eeb-aec0-86ba7e2dc2f6"), "EmailEnabled", "False" },
                    { new Guid("bd69a35d-2eec-49a5-a05f-cd2d6089c323"), "SmtpServer", "" },
                    { new Guid("4fa188c4-1892-40cb-bf63-8606df9fcbc8"), "SmtpServerPort", "0" },
                    { new Guid("0e51d7f2-d71a-4924-a5c9-363b9197b5b2"), "SmtpServerUser", "" },
                    { new Guid("ff4d59aa-3a8a-428f-8840-b75e5aba23b4"), "SmtpServerPassword", "" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("040569bc-3251-47d1-b51a-1a728c3d49ec"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("0e51d7f2-d71a-4924-a5c9-363b9197b5b2"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("1c629bb9-e897-4e6c-9017-300aef64d077"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("4fa188c4-1892-40cb-bf63-8606df9fcbc8"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("94010814-1ba1-4fca-8e57-a879ef51ba1a"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("a49f13c1-bd9a-41ac-90a4-4d9051b0cdec"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("af86f645-4209-4eeb-aec0-86ba7e2dc2f6"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("bd69a35d-2eec-49a5-a05f-cd2d6089c323"));

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: new Guid("ff4d59aa-3a8a-428f-8840-b75e5aba23b4"));
        }
    }
}
