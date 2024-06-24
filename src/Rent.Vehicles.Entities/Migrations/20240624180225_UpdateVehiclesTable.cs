using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehiclesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("39e02a68-9b2f-4638-869d-2a1b51145a9b"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("45df35af-ab37-4fa2-8324-f03806ddd48d"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("699b68db-2f0f-41ed-a414-b7fca8dcb5e1"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("a8118564-6c17-4e76-be3c-c8947fda30ad"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("d8a5a35f-c718-435b-a119-1ea2d679acb6"));

            migrationBuilder.AddColumn<bool>(
                name: "is_rented",
                schema: "vehicles",
                table: "vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "is_rented",
                schema: "vehicles",
                table: "vehicles");

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine" },
                values: new object[,]
                {
                    { new Guid("39e02a68-9b2f-4638-869d-2a1b51145a9b"), new DateTime(2024, 6, 24, 17, 50, 17, 247, DateTimeKind.Utc).AddTicks(3373), 30.0m, 7, 50.0m, 1.20m },
                    { new Guid("45df35af-ab37-4fa2-8324-f03806ddd48d"), new DateTime(2024, 6, 24, 17, 50, 17, 247, DateTimeKind.Utc).AddTicks(3426), 28.0m, 15, 50.0m, 1.40m },
                    { new Guid("699b68db-2f0f-41ed-a414-b7fca8dcb5e1"), new DateTime(2024, 6, 24, 17, 50, 17, 247, DateTimeKind.Utc).AddTicks(3452), 18.0m, 50, 50.0m, 1.0m },
                    { new Guid("a8118564-6c17-4e76-be3c-c8947fda30ad"), new DateTime(2024, 6, 24, 17, 50, 17, 247, DateTimeKind.Utc).AddTicks(3436), 22.0m, 30, 50.0m, 1.0m },
                    { new Guid("d8a5a35f-c718-435b-a119-1ea2d679acb6"), new DateTime(2024, 6, 24, 17, 50, 17, 247, DateTimeKind.Utc).AddTicks(3444), 20.0m, 45, 50.0m, 1.0m }
                });
        }
    }
}
