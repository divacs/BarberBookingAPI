using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedReminderFieldsToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ab5c5ec-77d4-4040-94ec-a9c1d63cee98");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dfeff327-4683-4057-8d51-87c875505260");

            migrationBuilder.AddColumn<string>(
                name: "ReminderJobId",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "530a88b6-aa2e-4050-b22f-e8cc8060abeb", null, "User", "USER" },
                    { "9a3dc33f-d4e4-4a95-bd76-87d8ab7257ac", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "530a88b6-aa2e-4050-b22f-e8cc8060abeb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a3dc33f-d4e4-4a95-bd76-87d8ab7257ac");

            migrationBuilder.DropColumn(
                name: "ReminderJobId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Appointments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ab5c5ec-77d4-4040-94ec-a9c1d63cee98", null, "Admin", "ADMIN" },
                    { "dfeff327-4683-4057-8d51-87c875505260", null, "User", "USER" }
                });
        }
    }
}
