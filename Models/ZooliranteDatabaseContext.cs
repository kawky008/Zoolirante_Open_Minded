using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zoolirante_Open_Minded.Models;

public partial class ZooliranteDatabaseContext : DbContext
{
    public ZooliranteDatabaseContext()
    {
    }

    public ZooliranteDatabaseContext(DbContextOptions<ZooliranteDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Merchandise> Merchandises { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=zoolirante-open-minded.database.windows.net;Initial Catalog=ZooliranteDatabase;Persist Security Info=True;User ID=itsmekento;Password=AJXgRGaQ3E9JkCg;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.AnimalId).HasName("PK__Animals__A21A730703C3EA3B");

            entity.HasIndex(e => e.Name, "IX_Animals_Name");

            entity.HasIndex(e => e.Region, "IX_Animals_Region");

            entity.HasIndex(e => e.Species, "IX_Animals_Species");

            entity.Property(e => e.ConservationStatus).HasMaxLength(20);
            entity.Property(e => e.ExhibitLocation).HasMaxLength(100);
            entity.Property(e => e.Habitat).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(50);
            entity.Property(e => e.Species).HasMaxLength(100);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C81096ED2C0C");

            entity.HasIndex(e => e.StartTime, "IX_Events_StartTime");

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartTime).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasMany(d => d.Animals).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventAnimal",
                    r => r.HasOne<Animal>().WithMany()
                        .HasForeignKey("AnimalId")
                        .HasConstraintName("FK_EventAnimals_Animal"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .HasConstraintName("FK_EventAnimals_Event"),
                    j =>
                    {
                        j.HasKey("EventId", "AnimalId");
                        j.ToTable("EventAnimals");
                    });
        });

        modelBuilder.Entity<Merchandise>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Merchand__B40CC6CDE5E5113D");

            entity.ToTable("Merchandise");

            entity.HasIndex(e => e.Category, "IX_Merch_Category");

            entity.HasIndex(e => e.Name, "IX_Merch_Name");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
