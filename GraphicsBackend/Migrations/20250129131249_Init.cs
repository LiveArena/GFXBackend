using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphicsBackend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectGraphics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JSONData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hide = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGraphics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ProjectThemeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JSONData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectThemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectThemes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectGraphics_ProjectId",
                table: "ProjectGraphics",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerId",
                table: "Projects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectThemeId",
                table: "Projects",
                column: "ProjectThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectThemes_ProjectId",
                table: "ProjectThemes",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectGraphics_Projects_ProjectId",
                table: "ProjectGraphics",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectThemes_ProjectThemeId",
                table: "Projects",
                column: "ProjectThemeId",
                principalTable: "ProjectThemes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectThemes_Projects_ProjectId",
                table: "ProjectThemes");

            migrationBuilder.DropTable(
                name: "ProjectGraphics");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ProjectThemes");
        }
    }
}
