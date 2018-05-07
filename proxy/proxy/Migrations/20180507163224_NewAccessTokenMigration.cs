using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace proxy.Migrations
{
    public partial class NewAccessTokenMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Auth",
                table: "AccessTokens",
                newName: "FedAuth1");

            migrationBuilder.AddColumn<string>(
                name: "FedAuth",
                table: "AccessTokens",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FedAuth",
                table: "AccessTokens");

            migrationBuilder.RenameColumn(
                name: "FedAuth1",
                table: "AccessTokens",
                newName: "Auth");
        }
    }
}
