using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("3d3eff16-f44a-48c8-9124-dd2146f7445f"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("3f0b41b2-d448-4f1a-878b-ab6554c9b9b3"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("56bf4d36-c9fb-46e0-80ac-1bf23b1bcb13"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("5ff3d14d-c145-40b6-bae4-938488506ddb"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("701aebde-0d7b-4533-8dc6-7cfa3b193f1f"));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "vehicles",
                table: "vehicles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "vehicles",
                table: "users",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "vehicles",
                table: "rents",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "vehicles",
                table: "rentalPlanes",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "events",
                table: "events",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                schema: "events",
                table: "commands",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine", "updated" },
                values: new object[,]
                {
                    { new Guid("0a2c4969-093b-44e6-908d-7bb8712ed431"), new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(965), 18.0m, 50, 50.0m, 1.0m, null },
                    { new Guid("9aeaca2f-d9c3-4a33-824a-c2ef92dd4355"), new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(910), 30.0m, 7, 50.0m, 1.20m, null },
                    { new Guid("c0269299-d465-42ef-820c-6b13eef29ee9"), new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(947), 28.0m, 15, 50.0m, 1.40m, null },
                    { new Guid("df107815-8295-43d3-a023-4adb387a8601"), new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(960), 20.0m, 45, 50.0m, 1.0m, null },
                    { new Guid("e3d5062d-ca48-40cb-b248-a9b7d47e0599"), new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(954), 22.0m, 30, 50.0m, 1.0m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("0a2c4969-093b-44e6-908d-7bb8712ed431"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("9aeaca2f-d9c3-4a33-824a-c2ef92dd4355"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("c0269299-d465-42ef-820c-6b13eef29ee9"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("df107815-8295-43d3-a023-4adb387a8601"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("e3d5062d-ca48-40cb-b248-a9b7d47e0599"));

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "vehicles",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "vehicles",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "vehicles",
                table: "rents");

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "vehicles",
                table: "rentalPlanes");

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "events",
                table: "events");

            migrationBuilder.DropColumn(
                name: "updated",
                schema: "events",
                table: "commands");

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine" },
                values: new object[,]
                {
                    { new Guid("3d3eff16-f44a-48c8-9124-dd2146f7445f"), new DateTime(2024, 6, 24, 18, 2, 24, 399, DateTimeKind.Utc).AddTicks(7679), 20.0m, 45, 50.0m, 1.0m },
                    { new Guid("3f0b41b2-d448-4f1a-878b-ab6554c9b9b3"), new DateTime(2024, 6, 24, 18, 2, 24, 399, DateTimeKind.Utc).AddTicks(7656), 28.0m, 15, 50.0m, 1.40m },
                    { new Guid("56bf4d36-c9fb-46e0-80ac-1bf23b1bcb13"), new DateTime(2024, 6, 24, 18, 2, 24, 399, DateTimeKind.Utc).AddTicks(7669), 22.0m, 30, 50.0m, 1.0m },
                    { new Guid("5ff3d14d-c145-40b6-bae4-938488506ddb"), new DateTime(2024, 6, 24, 18, 2, 24, 399, DateTimeKind.Utc).AddTicks(7690), 18.0m, 50, 50.0m, 1.0m },
                    { new Guid("701aebde-0d7b-4533-8dc6-7cfa3b193f1f"), new DateTime(2024, 6, 24, 18, 2, 24, 399, DateTimeKind.Utc).AddTicks(7580), 30.0m, 7, 50.0m, 1.20m }
                });
        }
    }
}
