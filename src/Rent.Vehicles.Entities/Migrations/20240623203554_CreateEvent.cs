using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class CreateEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_commands_saga_id",
                schema: "events",
                table: "commands",
                column: "saga_id");

            migrationBuilder.CreateTable(
                name: "events",
                schema: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    saga_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    status_type = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    serializer_type = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<byte[]>(type: "smallint[]", nullable: false)
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

            migrationBuilder.DropUniqueConstraint(
                name: "ak_commands_saga_id",
                schema: "events",
                table: "commands");
        }
    }
}
