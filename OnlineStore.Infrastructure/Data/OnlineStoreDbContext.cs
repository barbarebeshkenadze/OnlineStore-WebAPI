using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;

namespace OnlineStore.Infrastructure.Data
{
    public class OnlineStoreDbContext : DbContext
    {
        public OnlineStoreDbContext(
            DbContextOptions<OnlineStoreDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

         
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Description)
                    .HasMaxLength(500);

                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

               
                entity.Property(c => c.Description)
                    .HasMaxLength(300);
            });

     
            modelBuilder.Entity<Customer>(entity =>
            {
                
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                
                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(150);

               
                entity.Property(c => c.Phone)
                    .HasMaxLength(30);

               
                entity.HasIndex(c => c.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
               
                entity.HasKey(o => o.Id);

               
                entity.Property(o => o.OrderDate)
                    .IsRequired();

              
                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(o => o.TotalAmount)
                    .HasPrecision(18, 2);

               
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        
            modelBuilder.Entity<OrderItem>(entity =>
            {
               
                entity.HasKey(oi => oi.Id);

                
                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                
                entity.Property(oi => oi.UnitPrice)
                    .HasPrecision(18, 2);

                
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

               
                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
