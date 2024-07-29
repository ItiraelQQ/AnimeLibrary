using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddViewedListsToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ViewedAnimes",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ViewedMangas",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewedAnimes",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ViewedMangas",
                table: "AspNetUsers");
        }
    }
}
