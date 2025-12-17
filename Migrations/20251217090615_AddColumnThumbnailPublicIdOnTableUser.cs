using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bidify_be.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnThumbnailPublicIdOnTableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPublicId",
                table: "Products",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailPublicId",
                table: "Products");
        }
    }
}
