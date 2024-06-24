using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class CreateRentalPlanes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rentalPlanes",
                schema: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number_of_days = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<decimal>(type: "numeric", nullable: false),
                    pre_end_date_percentage_fine = table.Column<decimal>(type: "numeric", nullable: false),
                    post_end_date_fine = table.Column<decimal>(type: "numeric", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rental_planes", x => x.id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rentalPlanes",
                schema: "vehicles");
        }
    }
}
