using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherActivityMatcher.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateSlug = table.Column<string>(type: "TEXT", nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HourlyForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ValidTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Temperature2m = table.Column<double>(type: "REAL", nullable: false),
                    Windspeed10m = table.Column<double>(type: "REAL", nullable: false),
                    Precipitation = table.Column<double>(type: "REAL", nullable: false),
                    Cloudcover = table.Column<double>(type: "REAL", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourlyForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    LocationName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    WindowStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WindowEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Channel = table.Column<string>(type: "TEXT", nullable: false),
                    Read = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeatherConstraints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    TempMin = table.Column<double>(type: "REAL", nullable: true),
                    TempMax = table.Column<double>(type: "REAL", nullable: true),
                    WindMin = table.Column<double>(type: "REAL", nullable: true),
                    WindMax = table.Column<double>(type: "REAL", nullable: true),
                    CloudMin = table.Column<double>(type: "REAL", nullable: true),
                    CloudMax = table.Column<double>(type: "REAL", nullable: true),
                    PrecipMax = table.Column<double>(type: "REAL", nullable: true),
                    TimeStart = table.Column<string>(type: "TEXT", nullable: true),
                    TimeEnd = table.Column<string>(type: "TEXT", nullable: true),
                    DaysOfWeek = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherConstraints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherConstraints_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "Id", "Latitude", "LocationName", "Longitude" },
                values: new object[] { 1, 52.369999999999997, "Amsterdam", 4.9000000000000004 });

            migrationBuilder.CreateIndex(
                name: "IX_HourlyForecasts_ValidTime",
                table: "HourlyForecasts",
                column: "ValidTime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ActivityId",
                table: "Notifications",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherConstraints_ActivityId",
                table: "WeatherConstraints",
                column: "ActivityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HourlyForecasts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "WeatherConstraints");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
