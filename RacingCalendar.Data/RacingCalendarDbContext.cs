using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using RacingCalendar.Data.Models;

public class RacingCalendarDbContext : IdentityDbContext
{
    public RacingCalendarDbContext(DbContextOptions<RacingCalendarDbContext> options)
    : base(options)
    {
    }

    public DbSet<Series> Series { get; set; }
    public DbSet<Circuit> Circuits { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverRace> DriverRaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Race>()
            .HasOne(r => r.Series)
            .WithMany(s => s.Races)
        .HasForeignKey(r => r.SeriesId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Race>()
            .HasOne(r => r.Circuit)
            .WithMany(c => c.Races)
            .HasForeignKey(r => r.CircuitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Team)
            .WithMany(t => t.Drivers)
            .HasForeignKey(d => d.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DriverRace>()
        .HasKey(dr => new { dr.DriverId, dr.RaceId });

        modelBuilder.Entity<DriverRace>()
            .HasOne(dr => dr.Driver)
            .WithMany(d => d.RaceEntries)
            .HasForeignKey(dr => dr.DriverId);

        modelBuilder.Entity<DriverRace>()
            .HasOne(dr => dr.Race)
            .WithMany(r => r.DriverEntries)
            .HasForeignKey(dr => dr.RaceId);
    }
}
