﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TracerDemo.Data;
using TracerDemo.Model;

namespace TracerDemo.Migrations
{
    [DbContext(typeof(SqliteContext))]
    partial class SqliteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("RiotNet.Models.Champion", b =>
                {
                    b.Property<long>("Id");

                    b.Property<bool>("Active");

                    b.Property<bool>("BotEnabled");

                    b.Property<bool>("BotMmEnabled");

                    b.Property<bool>("FreeToPlay");

                    b.Property<bool>("RankedPlayEnabled");

                    b.HasKey("Id");

                    b.ToTable("Champion");
                });

            modelBuilder.Entity("RiotNet.Models.Summoner", b =>
                {
                    b.Property<long>("Id");

                    b.Property<long>("AccountId");

                    b.Property<string>("Name");

                    b.Property<int>("ProfileIconId");

                    b.Property<DateTime>("RevisionDate");

                    b.Property<long>("SummonerLevel");

                    b.HasKey("Id");

                    b.ToTable("Summoner");
                });

            modelBuilder.Entity("TracerDemo.Model.ChampionStats", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PlayerStatsId");

                    b.Property<long?>("championId");

                    b.Property<string>("championStatsId");

                    b.Property<string>("playerId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerStatsId");

                    b.HasIndex("championId");

                    b.HasIndex("championStatsId");

                    b.HasIndex("playerId");

                    b.ToTable("ChampionStats");
                });

            modelBuilder.Entity("TracerDemo.Model.PlayerStats", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("statsId");

                    b.HasKey("Id");

                    b.HasIndex("statsId");

                    b.ToTable("PlayerStats");
                });

            modelBuilder.Entity("TracerDemo.Model.Rol", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserId");

                    b.Property<string>("value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rol");
                });

            modelBuilder.Entity("TracerDemo.Model.Stats", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Asists");

                    b.Property<float>("CreepsAtTen");

                    b.Property<float>("CreepsDifAtTen");

                    b.Property<int>("Daths");

                    b.Property<float>("DeathShare");

                    b.Property<float>("FirstBlood");

                    b.Property<int>("Games");

                    b.Property<float>("KillParticipation");

                    b.Property<float>("KillShare");

                    b.Property<int>("Kills");

                    b.Property<int>("Minions");

                    b.Property<float>("MinionsMinute");

                    b.Property<float>("MinutesXMatch");

                    b.Property<float>("WardXmin");

                    b.Property<float>("WinRate");

                    b.HasKey("Id");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("TracerDemo.Model.Team", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LastUpdate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("TracerDemo.Model.TeamTracerPlayer", b =>
                {
                    b.Property<string>("TeamId");

                    b.Property<string>("TracerPlayerId");

                    b.Property<int>("Position");

                    b.HasKey("TeamId", "TracerPlayerId");

                    b.HasIndex("TracerPlayerId");

                    b.ToTable("TeamTracerPlayer");
                });

            modelBuilder.Entity("TracerDemo.Model.Todo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Completed");

                    b.Property<string>("Owner");

                    b.Property<string>("Task");

                    b.HasKey("Id");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("TracerDemo.Model.TracerPlayer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("StatsId");

                    b.Property<long?>("SummonerId");

                    b.HasKey("Id");

                    b.HasIndex("StatsId")
                        .IsUnique();

                    b.HasIndex("SummonerId");

                    b.ToTable("TracerPlayers");
                });

            modelBuilder.Entity("TracerDemo.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivationToken");

                    b.Property<DateTime?>("ActivationTokenExpiration");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailValidated");

                    b.Property<string>("FacebookId");

                    b.Property<DateTime>("LastSignin");

                    b.Property<int>("LockoutCount");

                    b.Property<DateTime?>("LockoutDateTime");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("RecoveryToken");

                    b.Property<DateTime?>("RecoveryTokenExpiration");

                    b.Property<string>("Salt");

                    b.Property<DateTime>("Updated");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TracerDemo.Model.ChampionStats", b =>
                {
                    b.HasOne("TracerDemo.Model.PlayerStats")
                        .WithMany("championStats")
                        .HasForeignKey("PlayerStatsId");

                    b.HasOne("RiotNet.Models.Champion", "champion")
                        .WithMany()
                        .HasForeignKey("championId");

                    b.HasOne("TracerDemo.Model.Stats", "championStats")
                        .WithMany()
                        .HasForeignKey("championStatsId");

                    b.HasOne("TracerDemo.Model.TracerPlayer", "player")
                        .WithMany()
                        .HasForeignKey("playerId");
                });

            modelBuilder.Entity("TracerDemo.Model.PlayerStats", b =>
                {
                    b.HasOne("TracerDemo.Model.Stats", "stats")
                        .WithMany()
                        .HasForeignKey("statsId");
                });

            modelBuilder.Entity("TracerDemo.Model.Rol", b =>
                {
                    b.HasOne("TracerDemo.Model.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TracerDemo.Model.TeamTracerPlayer", b =>
                {
                    b.HasOne("TracerDemo.Model.Team", "Team")
                        .WithMany("TeamsRelation")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TracerDemo.Model.TracerPlayer", "TracerPlayer")
                        .WithMany("TeamsRelation")
                        .HasForeignKey("TracerPlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TracerDemo.Model.TracerPlayer", b =>
                {
                    b.HasOne("TracerDemo.Model.PlayerStats", "Stats")
                        .WithOne("player")
                        .HasForeignKey("TracerDemo.Model.TracerPlayer", "StatsId");

                    b.HasOne("RiotNet.Models.Summoner", "Summoner")
                        .WithMany()
                        .HasForeignKey("SummonerId");
                });
#pragma warning restore 612, 618
        }
    }
}
