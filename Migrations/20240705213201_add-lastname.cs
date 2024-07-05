using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Iden.Migrations
{
    /// <inheritdoc />
    public partial class addlastname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9500977f-acd5-4c8d-9c04-1eb77b967b29");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "efa491e7-ec02-4ba4-8f45-98bde88267f0");

            migrationBuilder.AddColumn<string>(
                name: "lastName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "09375c71-bf05-4afa-945b-d2dd42645cf4", null, "User", "USER" },
                    { "f379e7e3-607e-454a-bf2b-ca02ae2111be", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09375c71-bf05-4afa-945b-d2dd42645cf4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f379e7e3-607e-454a-bf2b-ca02ae2111be");

            migrationBuilder.DropColumn(
                name: "lastName",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9500977f-acd5-4c8d-9c04-1eb77b967b29", null, "User", "USER" },
                    { "efa491e7-ec02-4ba4-8f45-98bde88267f0", null, "Admin", "ADMIN" }
                });
        }
    }
}
