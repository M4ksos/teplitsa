using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teplitsa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Greenhouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Greenhouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrowthPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CropName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Actuator",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Position_X = table.Column<int>(type: "INTEGER", nullable: false),
                    Position_Y = table.Column<int>(type: "INTEGER", nullable: false),
                    IsOn = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActuatorKind = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    GreenhouseId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actuator_Greenhouses_GreenhouseId",
                        column: x => x.GreenhouseId,
                        principalTable: "Greenhouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sensor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Position_X = table.Column<int>(type: "INTEGER", nullable: false),
                    Position_Y = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentValue = table.Column<double>(type: "REAL", nullable: false),
                    GreenhouseId = table.Column<int>(type: "INTEGER", nullable: true),
                    SensorKind = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    MinAllowed = table.Column<double>(type: "REAL", nullable: true),
                    MaxAllowed = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensor_Greenhouses_GreenhouseId",
                        column: x => x.GreenhouseId,
                        principalTable: "Greenhouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GrowthCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GreenhouseId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthCycles_Greenhouses_GreenhouseId",
                        column: x => x.GreenhouseId,
                        principalTable: "Greenhouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrowthCycles_GrowthPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "GrowthPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthStage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayFrom = table.Column<int>(type: "INTEGER", nullable: false),
                    DayTo = table.Column<int>(type: "INTEGER", nullable: false),
                    MinPh = table.Column<double>(type: "REAL", nullable: false),
                    MaxPh = table.Column<double>(type: "REAL", nullable: false),
                    MinHumidity = table.Column<double>(type: "REAL", nullable: false),
                    MaxHumidity = table.Column<double>(type: "REAL", nullable: false),
                    GrowthPlanId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthStage_GrowthPlans_GrowthPlanId",
                        column: x => x.GrowthPlanId,
                        principalTable: "GrowthPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Start = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    End = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TargetTemperature = table.Column<double>(type: "REAL", nullable: false),
                    Tolerance = table.Column<double>(type: "REAL", nullable: false),
                    LightOn = table.Column<bool>(type: "INTEGER", nullable: false),
                    GrowthStageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlot_GrowthStage_GrowthStageId",
                        column: x => x.GrowthStageId,
                        principalTable: "GrowthStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actuator_GreenhouseId",
                table: "Actuator",
                column: "GreenhouseId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCycles_GreenhouseId",
                table: "GrowthCycles",
                column: "GreenhouseId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthCycles_PlanId",
                table: "GrowthCycles",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthStage_GrowthPlanId",
                table: "GrowthStage",
                column: "GrowthPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_GreenhouseId",
                table: "Sensor",
                column: "GreenhouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_GrowthStageId",
                table: "TimeSlot",
                column: "GrowthStageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actuator");

            migrationBuilder.DropTable(
                name: "GrowthCycles");

            migrationBuilder.DropTable(
                name: "Sensor");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "Greenhouses");

            migrationBuilder.DropTable(
                name: "GrowthStage");

            migrationBuilder.DropTable(
                name: "GrowthPlans");
        }
    }
}
