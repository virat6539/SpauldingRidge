using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SpauldingRidge.Models;

namespace SpauldingRidge.Data;

public partial class SpauldingContext : DbContext
{
    public SpauldingContext()
    {
    }

    public SpauldingContext(DbContextOptions<SpauldingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersReturn> OrdersReturns { get; set; }

    public virtual DbSet<Product> Products { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.CustomerId).HasMaxLength(255);
            entity.Property(e => e.CustomerName).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasMaxLength(255);
            entity.Property(e => e.PostalCode).HasColumnName("Postal Code");
            entity.Property(e => e.Region).HasMaxLength(255);
            entity.Property(e => e.Segment).HasMaxLength(255);
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.ShipMode).HasMaxLength(255);
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<OrdersReturn>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Comments).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasMaxLength(255);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.SubCategory).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
