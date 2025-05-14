using Microsoft.EntityFrameworkCore;

namespace Wypożyczlania_sprzętu.Entities;

public class RentalDbContext : DbContext
{
    private readonly string _connectionstring = "Server=(localdb)\\MSSQLLocalDb;Database=WypożyczalniaSprzętuUczelnianego;TrustServerCertificate=True;MultipleActiveResultSets=true";
    public DbSet<Borrowing> Borrowings { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<FaultReport> FaultReports { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.SerialNumber)
            .IsUnique();
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Equipment)
            .WithMany(e => e.Reservations)
            .HasForeignKey(r => r.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Borrowing>()
            .HasOne(b => b.User)
            .WithMany(u => u.Borrowings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Borrowing>()
            .HasOne(b => b.Equipment)
            .WithMany(e => e.Borrowings)
            .HasForeignKey(b => b.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FaultReport>()
            .HasOne(fr => fr.User)
            .WithMany(u => u.FaultReports)
            .HasForeignKey(fr => fr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FaultReport>()
            .HasOne(fr => fr.Equipment)
            .WithMany(e => e.FaultReports)
            .HasForeignKey(fr => fr.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NotificationLog>()
            .HasOne(n => n.User)
            .WithMany(u => u.NotificationLogs)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionstring);
    }
}
