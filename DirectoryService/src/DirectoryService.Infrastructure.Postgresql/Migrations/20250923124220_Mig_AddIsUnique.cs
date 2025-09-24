using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class Mig_AddIsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_positions_name",
                schema: "DirectoryService",
                table: "positions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                schema: "DirectoryService",
                table: "locations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_name",
                schema: "DirectoryService",
                table: "departments",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_positions_name",
                schema: "DirectoryService",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                schema: "DirectoryService",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_departments_name",
                schema: "DirectoryService",
                table: "departments");
        }
    }
}
