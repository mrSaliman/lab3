﻿using lab3.Models;
using lab3.Views;
using Microsoft.EntityFrameworkCore;

namespace lab3;

public partial class AcmeDataContext : DbContext
{
    public AcmeDataContext()
    {
    }

    public AcmeDataContext(DbContextOptions<AcmeDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerTotalOrder> CustomerTotalOrders { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeTotalCostWeight> EmployeeTotalCostWeights { get; set; }

    public virtual DbSet<Furniture> Furnitures { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsView> OrderDetailsViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8AC00F230");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RepresentativeFirstName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeLastName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeMiddleName).HasMaxLength(255);
        });

        modelBuilder.Entity<CustomerTotalOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CustomerTotalOrders");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RepresentativeFirstName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeLastName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeMiddleName).HasMaxLength(255);
            entity.Property(e => e.TotalPurchases).HasColumnType("decimal(38, 4)");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1BB42DE48");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Education).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeTotalCostWeight>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EmployeeTotalCostWeight");

            entity.Property(e => e.EmployeeEducation).HasMaxLength(255);
            entity.Property(e => e.EmployeeFirstName).HasMaxLength(255);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeeLastName).HasMaxLength(255);
            entity.Property(e => e.EmployeeMiddleName).HasMaxLength(255);
            entity.Property(e => e.EmployeePosition).HasMaxLength(100);
            entity.Property(e => e.TotalCost).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.TotalWeight).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<Furniture>(entity =>
        {
            entity.HasKey(e => e.FurnitureId).HasName("PK__Furnitur__D4323505FFDAB4BF");

            entity.ToTable("Furniture");

            entity.Property(e => e.FurnitureId).HasColumnName("FurnitureID");
            entity.Property(e => e.FurnitureName).HasMaxLength(255);
            entity.Property(e => e.MaterialType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD5F186281F");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.DeliveryDate).HasColumnType("date");
            entity.Property(e => e.MaterialType).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReceivedByEmployeeId).HasColumnName("ReceivedByEmployeeID");
            entity.Property(e => e.SupplierName).HasMaxLength(255);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ReceivedByEmployee).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ReceivedByEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__Receiv__44FF419A");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF2EEEDECA");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.ResponsibleEmployeeId).HasColumnName("ResponsibleEmployeeID");
            entity.Property(e => e.SpecialDiscount).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Customer__3D5E1FD2");

            entity.HasOne(d => d.ResponsibleEmployee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ResponsibleEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Responsi__3E52440B");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CC07AF555");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.FurnitureId).HasColumnName("FurnitureID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Furniture).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FurnitureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Furni__4222D4EF");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__412EB0B6");
        });

        modelBuilder.Entity<OrderDetailsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrderDetailsView");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.FurnitureId).HasColumnName("FurnitureID");
            entity.Property(e => e.FurnitureMaterial).HasMaxLength(50);
            entity.Property(e => e.FurnitureName).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RepresentativeFirstName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeLastName).HasMaxLength(255);
            entity.Property(e => e.RepresentativeMiddleName).HasMaxLength(255);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(28, 4)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
