using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Migrations
{
    /// <inheritdoc />
    public partial class Add_Scheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FileService");

            migrationBuilder.RenameTable(
                name: "media_assets",
                newName: "media_assets",
                newSchema: "FileService");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "media_assets",
                schema: "FileService",
                newName: "media_assets");
        }
    }
}
