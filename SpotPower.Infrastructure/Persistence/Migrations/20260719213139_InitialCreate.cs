using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotPower.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuctionPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeliveryDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Period = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodStartUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PriceEurPerMwhSpain = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    PriceEurPerMwhPortugal = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionPrices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionPrices_DeliveryDate_Period",
                table: "AuctionPrices",
                columns: new[] { "DeliveryDate", "Period" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionPrices");
        }
    }
}
