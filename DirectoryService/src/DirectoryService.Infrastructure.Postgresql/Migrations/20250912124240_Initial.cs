﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DirectoryService");

            migrationBuilder.CreateTable(
                name: "locations",
                schema: "DirectoryService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    timezone = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    house = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                schema: "DirectoryService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_position", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "DirectoryService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    identifier = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    path = table.Column<string>(type: "character varying(9999)", maxLength: 9999, nullable: false),
                    depth = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                    table.ForeignKey(
                        name: "FK_departments_departments_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "DirectoryService",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_departments_locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "DirectoryService",
                        principalTable: "locations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "department_locations",
                schema: "DirectoryService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_locations_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "DirectoryService",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_locations_locations_location_id",
                        column: x => x.location_id,
                        principalSchema: "DirectoryService",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "department_positions",
                schema: "DirectoryService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_positions_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "DirectoryService",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_department_positions_positions_position_id",
                        column: x => x.position_id,
                        principalSchema: "DirectoryService",
                        principalTable: "positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_department_locations_department_id",
                schema: "DirectoryService",
                table: "department_locations",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_locations_location_id",
                schema: "DirectoryService",
                table: "department_locations",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_positions_department_id",
                schema: "DirectoryService",
                table: "department_positions",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_department_positions_position_id",
                schema: "DirectoryService",
                table: "department_positions",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_LocationId",
                schema: "DirectoryService",
                table: "departments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_departments_parent_id",
                schema: "DirectoryService",
                table: "departments",
                column: "parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_locations",
                schema: "DirectoryService");

            migrationBuilder.DropTable(
                name: "department_positions",
                schema: "DirectoryService");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "DirectoryService");

            migrationBuilder.DropTable(
                name: "positions",
                schema: "DirectoryService");

            migrationBuilder.DropTable(
                name: "locations",
                schema: "DirectoryService");
        }
    }
}
