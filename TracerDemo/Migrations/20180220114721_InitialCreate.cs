using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TracerDemo.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Champion",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    BotEnabled = table.Column<bool>(nullable: false),
                    BotMmEnabled = table.Column<bool>(nullable: false),
                    FreeToPlay = table.Column<bool>(nullable: false),
                    RankedPlayEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Champion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Asists = table.Column<int>(nullable: false),
                    CreepsAtTen = table.Column<float>(nullable: false),
                    CreepsDifAtTen = table.Column<float>(nullable: false),
                    Daths = table.Column<int>(nullable: false),
                    DeathShare = table.Column<float>(nullable: false),
                    FirstBlood = table.Column<float>(nullable: false),
                    Games = table.Column<int>(nullable: false),
                    KillParticipation = table.Column<float>(nullable: false),
                    KillShare = table.Column<float>(nullable: false),
                    Kills = table.Column<int>(nullable: false),
                    Minions = table.Column<int>(nullable: false),
                    MinionsMinute = table.Column<float>(nullable: false),
                    MinutesXMatch = table.Column<float>(nullable: false),
                    WardXmin = table.Column<float>(nullable: false),
                    WinRate = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Summoner",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProfileIconId = table.Column<int>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false),
                    SummonerLevel = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summoner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Owner = table.Column<string>(nullable: true),
                    Task = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ActivationToken = table.Column<string>(nullable: true),
                    ActivationTokenExpiration = table.Column<DateTime>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailValidated = table.Column<bool>(nullable: false),
                    FacebookId = table.Column<string>(nullable: true),
                    LastSignin = table.Column<DateTime>(nullable: false),
                    LockoutCount = table.Column<int>(nullable: false),
                    LockoutDateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RecoveryToken = table.Column<string>(nullable: true),
                    RecoveryTokenExpiration = table.Column<DateTime>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    statsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Stats_statsId",
                        column: x => x.statsId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rol_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TracerPlayers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TeamId = table.Column<string>(nullable: true),
                    statsId = table.Column<string>(nullable: true),
                    summonerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TracerPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TracerPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TracerPlayers_PlayerStats_statsId",
                        column: x => x.statsId,
                        principalTable: "PlayerStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TracerPlayers_Summoner_summonerId",
                        column: x => x.summonerId,
                        principalTable: "Summoner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChampionStats",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PlayerStatsId = table.Column<string>(nullable: true),
                    championId = table.Column<long>(nullable: true),
                    championStatsId = table.Column<string>(nullable: true),
                    playerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampionStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChampionStats_PlayerStats_PlayerStatsId",
                        column: x => x.PlayerStatsId,
                        principalTable: "PlayerStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChampionStats_Champion_championId",
                        column: x => x.championId,
                        principalTable: "Champion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChampionStats_Stats_championStatsId",
                        column: x => x.championStatsId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChampionStats_TracerPlayers_playerId",
                        column: x => x.playerId,
                        principalTable: "TracerPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChampionStats_PlayerStatsId",
                table: "ChampionStats",
                column: "PlayerStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChampionStats_championId",
                table: "ChampionStats",
                column: "championId");

            migrationBuilder.CreateIndex(
                name: "IX_ChampionStats_championStatsId",
                table: "ChampionStats",
                column: "championStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChampionStats_playerId",
                table: "ChampionStats",
                column: "playerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_statsId",
                table: "PlayerStats",
                column: "statsId");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_UserId",
                table: "Rol",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TracerPlayers_TeamId",
                table: "TracerPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TracerPlayers_statsId",
                table: "TracerPlayers",
                column: "statsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TracerPlayers_summonerId",
                table: "TracerPlayers",
                column: "summonerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampionStats");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "Champion");

            migrationBuilder.DropTable(
                name: "TracerPlayers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "Summoner");

            migrationBuilder.DropTable(
                name: "Stats");
        }
    }
}
