﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeLibrary.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullText",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullText",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "News");
        }
    }
}