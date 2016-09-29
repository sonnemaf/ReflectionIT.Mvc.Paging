using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SampleApp.Models.Database {
    public partial class NorthwindContext : DbContext {

#if DEBUG

        // This constructor is only used for scaffolding controllers
        public NorthwindContext() : base(
            new DbContextOptionsBuilder<NorthwindContext>()
            .UseSqlServer("Server=.\\sqlexpress;Database=Northwind;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Option‌​s) {
        }
#endif

        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //    optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=Northwind;Trusted_Connection=True");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Categories>(entity => {
                entity.HasIndex(e => e.CategoryName)
                    .HasName("CategoryName");
            });

            modelBuilder.Entity<CustomerCustomerDemo>(entity => {
                entity.HasKey(e => new { e.CustomerId, e.CustomerTypeId })
                    .HasName("PK_CustomerCustomerDemo");
            });

            modelBuilder.Entity<Customers>(entity => {
                entity.HasIndex(e => e.City)
                    .HasName("City");

                entity.HasIndex(e => e.CompanyName)
                    .HasName("CompanyName");

                entity.HasIndex(e => e.PostalCode)
                    .HasName("PostalCode");

                entity.HasIndex(e => e.Region)
                    .HasName("Region");
            });

            modelBuilder.Entity<EmployeeTerritories>(entity => {
                entity.HasKey(e => new { e.EmployeeId, e.TerritoryId })
                    .HasName("PK_EmployeeTerritories");
            });

            modelBuilder.Entity<Employees>(entity => {
                entity.HasIndex(e => e.LastName)
                    .HasName("LastName");

                entity.HasIndex(e => e.PostalCode)
                    .HasName("PostalCode");
            });

            modelBuilder.Entity<OrderDetails>(entity => {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PK_Order_Details");

                entity.HasIndex(e => e.OrderId)
                    .HasName("OrdersOrder_Details");

                entity.HasIndex(e => e.ProductId)
                    .HasName("ProductsOrder_Details");

                entity.Property(e => e.Discount).HasDefaultValueSql("0");

                entity.Property(e => e.Quantity).HasDefaultValueSql("1");

                entity.Property(e => e.UnitPrice).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Orders>(entity => {
                entity.HasIndex(e => e.CustomerId)
                    .HasName("CustomersOrders");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("EmployeesOrders");

                entity.HasIndex(e => e.OrderDate)
                    .HasName("OrderDate");

                entity.HasIndex(e => e.ShipPostalCode)
                    .HasName("ShipPostalCode");

                entity.HasIndex(e => e.ShipVia)
                    .HasName("ShippersOrders");

                entity.HasIndex(e => e.ShippedDate)
                    .HasName("ShippedDate");

                entity.Property(e => e.Freight).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Products>(entity => {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("CategoryID");

                entity.HasIndex(e => e.ProductName)
                    .HasName("ProductName")
                    .IsUnique();

                entity.HasIndex(e => e.SupplierId)
                    .HasName("SuppliersProducts");

                entity.Property(e => e.Discontinued).HasDefaultValueSql("0");

                entity.Property(e => e.ReorderLevel).HasDefaultValueSql("0");

                entity.Property(e => e.UnitPrice).HasDefaultValueSql("0");

                entity.Property(e => e.UnitsInStock).HasDefaultValueSql("0");

                entity.Property(e => e.UnitsOnOrder).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Region>(entity => {
                entity.Property(e => e.RegionId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Suppliers>(entity => {
                entity.HasIndex(e => e.CompanyName)
                    .HasName("CompanyName");

                entity.HasIndex(e => e.PostalCode)
                    .HasName("PostalCode");
            });
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<CustomerCustomerDemo> CustomerCustomerDemo { get; set; }
        public virtual DbSet<CustomerDemographics> CustomerDemographics { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<EmployeeTerritories> EmployeeTerritories { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Shippers> Shippers { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<Territories> Territories { get; set; }
    }
}