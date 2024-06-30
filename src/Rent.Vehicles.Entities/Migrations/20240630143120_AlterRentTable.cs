using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AlterRentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "vehicles",
                table: "rents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine", "updated" },
                values: new object[,]
                {
                    { new Guid("09122534-816b-43b3-bd08-51f7aa392011"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6974), 18.0m, 50, 50.0m, 1.0m, null },
                    { new Guid("23f77fef-715e-4b5b-810f-7aa4eb2bc32d"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6908), 30.0m, 7, 50.0m, 1.20m, null },
                    { new Guid("3cf83b51-5a42-4e02-92d0-3be9669d7aee"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6955), 28.0m, 15, 50.0m, 1.40m, null },
                    { new Guid("7a605b65-512b-48a1-824a-660fff6d15a3"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6968), 20.0m, 45, 50.0m, 1.0m, null },
                    { new Guid("c218db2a-9d3e-40f0-b261-1eca059b5bef"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6962), 22.0m, 30, 50.0m, 1.0m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("09122534-816b-43b3-bd08-51f7aa392011"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("23f77fef-715e-4b5b-810f-7aa4eb2bc32d"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("3cf83b51-5a42-4e02-92d0-3be9669d7aee"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("7a605b65-512b-48a1-824a-660fff6d15a3"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("c218db2a-9d3e-40f0-b261-1eca059b5bef"));

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "vehicles",
                table: "rents");

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
    }
}
