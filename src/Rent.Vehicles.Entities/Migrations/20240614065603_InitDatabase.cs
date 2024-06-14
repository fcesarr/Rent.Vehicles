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
                    data = table.Column<byte[]>(type: "smallint[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commands", x => x.id);
                    table.UniqueConstraint("ak_commands_saga_id", x => x.saga_id);
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
                    table.PrimaryKey("PK_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    saga_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    status_type = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_events_commands_saga_id",
                        column: x => x.saga_id,
                        principalSchema: "events",
                        principalTable: "commands",
                        principalColumn: "saga_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehiclesForSpecificYear",
                schema: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehiclesForSpecificYear", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicles_for_specific_year_vehicles_id",
                        column: x => x.id,
                        principalSchema: "vehicles",
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_events_saga_id",
                schema: "events",
                table: "events",
                column: "saga_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events",
                schema: "events");

            migrationBuilder.DropTable(
                name: "vehiclesForSpecificYear",
                schema: "vehicles");

            migrationBuilder.DropTable(
                name: "commands",
                schema: "events");

            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "vehicles");
        }
    }
}
