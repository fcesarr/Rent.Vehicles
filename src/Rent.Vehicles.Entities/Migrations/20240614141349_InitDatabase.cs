using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "events");

            migrationBuilder.EnsureSchema(
                name: "vehicles");

            migrationBuilder.CreateTable(
                name: "commands",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    saga_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    serializer_type = table.Column<int>(type: "integer", nullable: false),
                    entity_type = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<byte[]>(type: "smallint[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                schema: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    license_plate = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commands",
                schema: "events");

            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "vehicles");
        }
    }
}
