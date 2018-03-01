using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TracerDemo.Model;

namespace TracerDemo.Data
{
    public class SqliteContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TracerPlayer> TracerPlayers {get; set;}
        public DbSet<Todo> Todos { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = "User ID=mbirljvqycbews;Password=ad6396cb46a7882d6ee2a1423a2c34a50aa50075208c39d1a8b9196a628ab1cf;Host=ec2-54-247-95-125.eu-west-1.compute.amazonaws.com;Port=5432;Database=d9i9f6396rilun;Pooling=true;Use SSL Stream=True;SSL Mode=Require;TrustServerCertificate=True;";

            optionsBuilder.UseNpgsql(connectionString: connString);
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamTracerPlayer>()
                .HasKey(pc => new { pc.TeamId, pc.TracerPlayerId });

            modelBuilder.Entity<TeamTracerPlayer>()
                .HasOne(pc => pc.Team)
                .WithMany(p => p.TeamsRelation)
                .HasForeignKey(pc => pc.TeamId);

            modelBuilder.Entity<TeamTracerPlayer>()
                .HasOne(pc => pc.TracerPlayer)
                .WithMany(c => c.TeamsRelation)
                .HasForeignKey(pc => pc.TracerPlayerId);
        }

    }
}
