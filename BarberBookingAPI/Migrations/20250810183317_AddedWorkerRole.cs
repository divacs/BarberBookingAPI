using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "530a88b6-aa2e-4050-b22f-e8cc8060abeb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a3dc33f-d4e4-4a95-bd76-87d8ab7257ac");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4e945e75-6c72-46e3-b996-9f7a8746ab6f", null, "Admin", "ADMIN" },
                    { "ae8fd584-cbb6-4181-b191-95e4da0feb74", null, "User", "USER" },
                    { "e649857d-355a-4fa7-9236-2e30a6cd5aea", null, "Worker", "WORKER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e945e75-6c72-46e3-b996-9f7a8746ab6f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ae8fd584-cbb6-4181-b191-95e4da0feb74");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e649857d-355a-4fa7-9236-2e30a6cd5aea");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "530a88b6-aa2e-4050-b22f-e8cc8060abeb", null, "User", "USER" },
                    { "9a3dc33f-d4e4-4a95-bd76-87d8ab7257ac", null, "Admin", "ADMIN" }
                });
        }
    }
}
