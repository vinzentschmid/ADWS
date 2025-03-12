using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceType = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: false),
                    DeviceDescription = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Aquarium = table.Column<string>(type: "text", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<int>(type: "integer", nullable: true),
                    SlaveID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DataPoints",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DatapointType = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataType = table.Column<int>(type: "integer", nullable: false),
                    Offset = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    FK_Device = table.Column<int>(type: "integer", nullable: false),
                    TopicName = table.Column<string>(type: "text", nullable: true),
                    RegisterType = table.Column<int>(type: "integer", nullable: true),
                    ReadingType = table.Column<int>(type: "integer", nullable: true),
                    Register = table.Column<int>(type: "integer", nullable: true),
                    RegisterCount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DataPoints_Devices_FK_Device",
                        column: x => x.FK_Device,
                        principalTable: "Devices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BinarySamples",
                columns: table => new
                {
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FK_Datapoint = table.Column<int>(type: "integer", nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinarySamples", x => new { x.FK_Datapoint, x.Time, x.CreationDate });
                    table.ForeignKey(
                        name: "FK_BinarySamples_DataPoints_FK_Datapoint",
                        column: x => x.FK_Datapoint,
                        principalTable: "DataPoints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NumericSamples",
                columns: table => new
                {
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FK_Datapoint = table.Column<int>(type: "integer", nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericSamples", x => new { x.FK_Datapoint, x.Time, x.CreationDate });
                    table.ForeignKey(
                        name: "FK_NumericSamples_DataPoints_FK_Datapoint",
                        column: x => x.FK_Datapoint,
                        principalTable: "DataPoints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BinarySamples_Time_FK_Datapoint",
                table: "BinarySamples",
                columns: new[] { "Time", "FK_Datapoint" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_DataPoints_FK_Device",
                table: "DataPoints",
                column: "FK_Device");

            migrationBuilder.CreateIndex(
                name: "IX_NumericSamples_Time_FK_Datapoint",
                table: "NumericSamples",
                columns: new[] { "Time", "FK_Datapoint" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinarySamples");

            migrationBuilder.DropTable(
                name: "NumericSamples");

            migrationBuilder.DropTable(
                name: "DataPoints");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
