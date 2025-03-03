using EventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;


namespace EventManagementSystem.Data
{

    public class EMSDbContext : DbContext
    {
        public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options)
        {
        }
        
        public DbSet<AppMaster> AppMasters { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<CityTownVillage> CityTownVillages { get; set; }
        public DbSet<EventsMaster> EventMasters { get; set; }
        public DbSet<GuestList> GuestLists { get; set; }
        public DbSet<EventGuestMapping> EventGuestMapping { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CityTownVillage>()
                .HasOne(d=>d.District)
                .WithMany(ctv=>ctv.CityTownVillages)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<District>()
                .HasOne(s=>s.State)
                .WithMany(d=>d.Districts)
                .HasForeignKey(s => s.StateId)
                .OnDelete(DeleteBehavior.Cascade);

                 modelBuilder.Entity<State>()
               .HasOne(s => s.Country)
               .WithMany(c => c.States)
               .HasForeignKey(s => s.CountryId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventGuestMapping>()
                .HasOne<EventsMaster>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<EventGuestMapping>()
                .HasOne<GuestList>()
                .WithMany()
                .HasForeignKey(g => g.GuestId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<EventsMaster>()
                .HasOne<AppMaster>()
                .WithMany()
                .HasForeignKey(a => a.AppID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GuestList>()
                .HasOne(ctv=>ctv.CityTownVillage)
                .WithMany(g=>g.GuestLists)
                .HasForeignKey(a => a.AppID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GuestList>()
                .HasOne<CityTownVillage>()
                .WithMany()
                .HasForeignKey(c => c.CityTownVillageID)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
