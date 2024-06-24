using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class CreateRents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("282ff9de-7436-4875-82da-2ca1b894f08f"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("2860ce87-765a-4df7-9c9b-00fb02d49265"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("68b5779a-27f9-4e35-949c-d66f1915701b"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("b664a818-09a2-4ac0-b58b-820624f6adb0"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("b96cac96-d3cb-40ae-a8de-0c67f08400ae"));

            migrationBuilder.RenameColumn(
                name: "cost",
                schema: "vehicles",
                table: "rentalPlanes",
                newName: "daily_cost");

            migrationBuilder.CreateTable(
                name: "rents",
                schema: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number_of_days = table.Column<int>(type: "integer", nullable: false),
                    daily_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    estimated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    pre_end_date_percentage_fine = table.Column<decimal>(type: "numeric", nullable: false),
                    post_end_date_fine = table.Column<decimal>(type: "numeric", nullable: false),
                    cost = table.Column<decimal>(type: "numeric", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rents", x => x.id);
                    table.ForeignKey(
                        name: "fk_rents_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "vehicles",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rents_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalSchema: "vehicles",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_rents_user_id",
                schema: "vehicles",
                table: "rents",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_rents_vehicle_id",
                schema: "vehicles",
                table: "rents",
                column: "vehicle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rents",
                schema: "vehicles");

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

            migrationBuilder.RenameColumn(
                name: "daily_cost",
                schema: "vehicles",
                table: "rentalPlanes",
                newName: "cost");

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "cost", "created", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine" },
                values: new object[,]
                {
                    { new Guid("282ff9de-7436-4875-82da-2ca1b894f08f"), 30.0m, new DateTime(2024, 6, 24, 17, 32, 39, 755, DateTimeKind.Utc).AddTicks(708), 7, 50.0m, 1.20m },
                    { new Guid("2860ce87-765a-4df7-9c9b-00fb02d49265"), 18.0m, new DateTime(2024, 6, 24, 17, 32, 39, 755, DateTimeKind.Utc).AddTicks(811), 50, 50.0m, 1.0m },
                    { new Guid("68b5779a-27f9-4e35-949c-d66f1915701b"), 28.0m, new DateTime(2024, 6, 24, 17, 32, 39, 755, DateTimeKind.Utc).AddTicks(778), 15, 50.0m, 1.40m },
                    { new Guid("b664a818-09a2-4ac0-b58b-820624f6adb0"), 20.0m, new DateTime(2024, 6, 24, 17, 32, 39, 755, DateTimeKind.Utc).AddTicks(801), 45, 50.0m, 1.0m },
                    { new Guid("b96cac96-d3cb-40ae-a8de-0c67f08400ae"), 22.0m, new DateTime(2024, 6, 24, 17, 32, 39, 755, DateTimeKind.Utc).AddTicks(790), 30, 50.0m, 1.0m }
                });
        }
    }
}
