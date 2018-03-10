using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DataStorageService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregateDataPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConversionKey = table.Column<double>(nullable: false),
                    RackCoordinateX = table.Column<int>(nullable: false),
                    RackCoordinateY = table.Column<int>(nullable: false),
                    RackIdentifier = table.Column<int>(nullable: false),
                    RawIntensity = table.Column<int>(nullable: false),
                    RoomNumber = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateDataPoints", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregateDataPoints");
        }
    }
}
