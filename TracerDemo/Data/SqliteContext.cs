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
            optionsBuilder.UseSqlite("Data Source=trace.db");
            
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
