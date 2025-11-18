using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class cascade_delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "update_at",
                schema: "DirectoryService",
                table: "positions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "update_at",
                schema: "DirectoryService",
                table: "locations",
                newName: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DirectoryService",
                table: "positions",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "DirectoryService",
                table: "locations",
                newName: "update_at");
        }
    }
}
