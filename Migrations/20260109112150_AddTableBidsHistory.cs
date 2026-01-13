using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bidify_be.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBidsHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BidsHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuctionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidsHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BidsHistory_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BidsHistory_AuctionId",
                table: "BidsHistory",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_BidsHistory_AuctionId_CreatedAt",
                table: "BidsHistory",
                columns: new[] { "AuctionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BidsHistory_UserId",
                table: "BidsHistory",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidsHistory");
        }
    }
}
