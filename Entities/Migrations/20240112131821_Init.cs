using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Core_8_0_Countries",
                columns: table => new
                {
                    CountryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_8_0_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Core_8_0_Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_8_0_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "Core_8_0_Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CountryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_8_0_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Core_8_0_Cities_Core_8_0_Countries_CountryID",
                        column: x => x.CountryID,
                        principalTable: "Core_8_0_Countries",
                        principalColumn: "CountryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_8_0_CityLanguages",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_8_0_CityLanguages", x => new { x.CityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Core_8_0_CityLanguages_Core_8_0_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Core_8_0_Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Core_8_0_CityLanguages_Core_8_0_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Core_8_0_Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_8_0_PointsOfInterest",
                columns: table => new
                {
                    PointOfInterestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointOfInterestName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PointOfInterestDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_8_0_PointsOfInterest", x => x.PointOfInterestId);
                    table.ForeignKey(
                        name: "FK_Core_8_0_PointsOfInterest_Core_8_0_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Core_8_0_Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Core_8_0_Cities_CountryID",
                table: "Core_8_0_Cities",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_Core_8_0_CityLanguages_LanguageId",
                table: "Core_8_0_CityLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_8_0_PointsOfInterest_CityId",
                table: "Core_8_0_PointsOfInterest",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Core_8_0_CityLanguages");

            migrationBuilder.DropTable(
                name: "Core_8_0_PointsOfInterest");

            migrationBuilder.DropTable(
                name: "Core_8_0_Languages");

            migrationBuilder.DropTable(
                name: "Core_8_0_Cities");

            migrationBuilder.DropTable(
                name: "Core_8_0_Countries");
        }
    }
}
