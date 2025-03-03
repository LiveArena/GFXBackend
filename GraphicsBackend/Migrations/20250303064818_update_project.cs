using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphicsBackend.Migrations
{
    /// <inheritdoc />
    public partial class update_project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JSONData",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JSONData",
                table: "Projects");
        }
    }
}
