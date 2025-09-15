using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class Mig_VOToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_locations_LocationId",
                schema: "DirectoryService",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "IX_departments_LocationId",
                schema: "DirectoryService",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "DirectoryService",
                table: "departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                schema: "DirectoryService",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_LocationId",
                schema: "DirectoryService",
                table: "departments",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_locations_LocationId",
                schema: "DirectoryService",
                table: "departments",
                column: "LocationId",
                principalSchema: "DirectoryService",
                principalTable: "locations",
                principalColumn: "id");
        }
    }
}
