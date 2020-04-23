using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class RemoveSiteAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parameters",
                keyColumn: "Id",
                keyValue: new Guid("24d0dbb4-386c-4e80-880d-72ac606c748e"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "Id", "Type", "Value" },
                values: new object[] { new Guid("24d0dbb4-386c-4e80-880d-72ac606c748e"), (ushort)2, "http://127.0.0.1:5000" });
        }
    }
}
