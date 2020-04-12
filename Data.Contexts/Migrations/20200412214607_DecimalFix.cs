using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeebReader.Data.Contexts.Migrations
{
    public partial class DecimalFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("1d105f2e-0b32-42dc-9fb5-d74cc1a81b12"));

            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("3d690373-9ea5-48bb-94c1-050f3825b833"));

            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("bc024453-6ebb-4da1-a206-17ea4b252a55"));

            migrationBuilder.AlterColumn<decimal>(
                "Number",
                "Chapters",
                "DECIMAL(5,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL");

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("b2f7adc5-9090-417c-bbc1-805071fc7a81"), (ushort)0, "WeebReader" });

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("27c51234-bf33-40de-86db-1941c8622a73"), (ushort)1, "We read weebs." });

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("24d0dbb4-386c-4e80-880d-72ac606c748e"), (ushort)2, "http://127.0.0.1:5000" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("24d0dbb4-386c-4e80-880d-72ac606c748e"));

            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("27c51234-bf33-40de-86db-1941c8622a73"));

            migrationBuilder.DeleteData(
                "Parameters",
                "Id",
                new Guid("b2f7adc5-9090-417c-bbc1-805071fc7a81"));

            migrationBuilder.AlterColumn<decimal>(
                "Number",
                "Chapters",
                "DECIMAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(5,1)");

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("bc024453-6ebb-4da1-a206-17ea4b252a55"), (ushort)0, "WeebReader" });

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("3d690373-9ea5-48bb-94c1-050f3825b833"), (ushort)1, "We read weebs." });

            migrationBuilder.InsertData(
                "Parameters",
                new[] { "Id", "Type", "Value" },
                new object[] { new Guid("1d105f2e-0b32-42dc-9fb5-d74cc1a81b12"), (ushort)2, "http://127.0.0.1:5000" });
        }
    }
}
