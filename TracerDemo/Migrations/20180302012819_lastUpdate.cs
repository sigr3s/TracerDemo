using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TracerDemo.Migrations
{
    public partial class lastUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Minutes",
                table: "Stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "WardKillXmin",
                table: "Stats",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Wards",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardsKilled",
                table: "Stats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "Stats",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "WardKillXmin",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Wards",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "WardsKilled",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Stats");
        }
    }
}
