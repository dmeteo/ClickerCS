﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpClicker.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");
        }
    }
}
