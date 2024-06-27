using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AlterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                schema: "vehicles",
                table: "vehicles",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                schema: "vehicles",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                schema: "events",
                table: "events",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                schema: "events",
                table: "commands",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created",
                schema: "vehicles",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "created",
                schema: "vehicles",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created",
                schema: "events",
                table: "events");

            migrationBuilder.DropColumn(
                name: "created",
                schema: "events",
                table: "commands");
        }
    }
}
