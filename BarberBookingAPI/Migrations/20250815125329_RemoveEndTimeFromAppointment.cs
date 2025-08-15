using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEndTimeFromAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3bfa9a6e-4897-4395-acd7-0b1ba741e474", null, "Admin", "ADMIN" },
                    { "5ae5c4c0-ff98-407b-8b99-94ca20af6b2f", null, "Worker", "WORKER" },
                    { "d598eac7-55ec-4862-8d53-356cb52d7a64", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3bfa9a6e-4897-4395-acd7-0b1ba741e474");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ae5c4c0-ff98-407b-8b99-94ca20af6b2f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d598eac7-55ec-4862-8d53-356cb52d7a64");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Appointments");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
