﻿using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Model;
using System.Reflection.Metadata;

namespace SPG_Fachtheorie.Aufgabe1.Infrastructure
{
    public class AppointmentContext : DbContext
    {
        // TODO: Add your DbSets here
        public DbSet<CashDesk> CashDesks => Set<CashDesk>();
        public DbSet<Cashier> Cashiers => Set<Cashier>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentItem> PaymentItems => Set<PaymentItem>();

        public AppointmentContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: Add your configuration here
            modelBuilder.Entity<Employee>()
                .OwnsOne(e => e.Address);
            modelBuilder.Entity<Employee>()
                .HasDiscriminator(b => b.Type);
        }
    }
}