using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace KarimStore.Models
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model19")
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<InvoiceHistory> InvoiceHistories { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<ProductPic> ProductPics { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProfilePic> ProfilePics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            modelBuilder.Entity<Cart>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 4);

            modelBuilder.Entity<InvoiceHistory>()
                .Property(e => e.Total)
                .HasPrecision(10, 4);

            modelBuilder.Entity<InvoiceHistory>()
                .HasMany(e => e.Carts)
                .WithOptional(e => e.InvoiceHistory)
                .HasForeignKey(e => e.InvoiceHistoryID);

            modelBuilder.Entity<InvoiceHistory>()
                .HasMany(e => e.Invoices)
                .WithOptional(e => e.InvoiceHistory)
                .HasForeignKey(e => e.InvoiceHistoryID);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);
        }
    }
}
