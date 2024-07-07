using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Iden.Migrations
{
    /// <inheritdoc />
    public partial class forOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aeac186f-01ed-4d5d-80c0-435e9eb9f964");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0aee9e3-f9dc-4f5c-81bd-4e666f5f3bcf");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "52d55d48-e0af-4e45-a2c2-86f9c10b8c38", null, "User", "USER" },
                    { "e7914031-57f7-4117-a836-9b74bf2c80a7", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "52d55d48-e0af-4e45-a2c2-86f9c10b8c38");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7914031-57f7-4117-a836-9b74bf2c80a7");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "aeac186f-01ed-4d5d-80c0-435e9eb9f964", null, "User", "USER" },
                    { "c0aee9e3-f9dc-4f5c-81bd-4e666f5f3bcf", null, "Admin", "ADMIN" }
                });
        }
    }
}
