using BulkyWeb.Models;
using BulkyWeb.Models.Identities;
using BulkyWeb.Models.Mocks;
using BulkyWeb.Models.Orders;
using BulkyWeb.Models.Payments;
using BulkyWeb.Models.Permissions;
using BulkyWeb.Models.RefreshTokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //old
        //public DbSet<User> Users { get; set; }
        //public DbSet<Project> Projects { get; set; }
        //public DbSet<TaskItem> TaskItems { get; set; }
        //public DbSet<Comment> Comments { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ExternalDataEntry> ExternalDataEntries { get; set; }
        /// <summary>
        /// //These tables are commented for  their confliction with AspNetUser identity package
        /// </summary>
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<CustomerCustomerDemo> CustomerCustomerDemo { get; set; }
        //public DbSet<CustomerDemographic> CustomerDemographics { get; set; }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
        //public DbSet<Region> Region { get; set; }
        public DbSet<Territory> Territories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Comment>()
            //   .HasOne(c => c.User)
            //   .WithMany()
            //   .HasForeignKey(c => c.UserId)
            //   .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.TaskItem)
            //    .WithMany()
            //    .HasForeignKey(c => c.TaskItemId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>().ToTable("RolePermission").HasKey(rp => new { rp.RoleID, rp.PermissionID });
            modelBuilder.Entity<Permission>().ToTable("Permission").HasKey(rp => rp.Id);



            modelBuilder.Entity<OrderDetail>()
                    .HasKey(od => new { od.OrderID, od.ProductID })

                    ;

            modelBuilder.Entity<OrderDetail>()
                .ToTable("Order Details");

            modelBuilder.Entity<ExternalDataEntry>()
                .ToTable("ExternalDataEntry");


            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID).IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID).IsRequired();


            // Product -> Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Product -> Supplier
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(x => x.OrderDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(x => x.RequiredDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Order -> Customer
            //modelBuilder.Entity<Order>()
            //    .HasOne(o => o.Customer)
            //    .WithMany(c => c.Orders)
            //    .HasForeignKey(o => o.CustomerID)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Order -> Employee
            //modelBuilder.Entity<Order>()
            //    .HasOne(o => o.Employee)
            //    .WithMany(e => e.Orders)
            //    .HasForeignKey(o => o.EmployeeID)
            //    .OnDelete(DeleteBehavior.Cascade);



            // Order -> Shipper
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Shipper)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ShipVia)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<RefreshToken>()
                .HasIndex(x => x.Token)
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
    .Property(x => x.RowVersion)
    .IsRowVersion();


            // Territory -> Region
            modelBuilder.Entity<Territory>()
                .HasOne(t => t.Region)
                .WithMany(r => r.Territories)
                .HasForeignKey(t => t.RegionID)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(p => p.TotalAfterDiscount)
.HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(p => p.TotalBeforeDiscount)
.HasPrecision(18, 2);

            //// EmployeeTerritories (Many-to-Many)
            //modelBuilder.Entity<EmployeeTerritory>().ToTable("EmployeeTerritories")
            //    .HasKey(et => new { et.EmployeeID, et.TerritoryID });


            //modelBuilder.Entity<EmployeeTerritory>()
            //    .HasOne(et => et.Employee)
            //    .WithMany(e => e.EmployeeTerritories)
            //    .HasForeignKey(et => et.EmployeeID);

            //modelBuilder.Entity<EmployeeTerritory>()
            //    .HasOne(et => et.Territory)
            //    .WithMany(t => t.EmployeeTerritories)
            //    .HasForeignKey(et => et.TerritoryID);


            // CustomerCustomerDemo (Many-to-Many)
            //modelBuilder.Entity<CustomerCustomerDemo>()
            //    .HasKey(ccd => new { ccd.CustomerID, ccd.CustomerTypeID });

            //modelBuilder.Entity<CustomerCustomerDemo>()
            //    .HasOne(ccd => ccd.Customer)
            //    .WithMany(c => c.CustomerCustomerDemos)
            //    .HasForeignKey(ccd => ccd.CustomerID);

            //modelBuilder.Entity<CustomerCustomerDemo>()
            //    .HasOne(ccd => ccd.CustomerDemographic)
            //    .WithMany(cd => cd.CustomerCustomerDemos)
            //    .HasForeignKey(ccd => ccd.CustomerTypeID);



            modelBuilder.Entity<Permission>().HasData(
    new Permission { Id = 1, Name = "Category.Create" },
    new Permission { Id = 2, Name = "Category.Read" },
    new Permission { Id = 3, Name = "Category.Delete" }
);

            var adminRoleId = "11111111-1111-1111-1111-111111111111";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            );
            modelBuilder.Entity<RolePermission>().HasData(
    new RolePermission { RoleID = adminRoleId, PermissionID = 1 },
    new RolePermission { RoleID = adminRoleId, PermissionID = 2 },
    new RolePermission { RoleID = adminRoleId, PermissionID = 3 }
);

            //modelBuilder.Entity<Category>().HasData(
            //       new Category { Category_Id = 1, CategoryName = "Action", DisplayOrder = 1 },
            //new Category { Category_Id = 2, CategoryName = "Adventure", DisplayOrder = 2 },
            //new Category { Category_Id = 3, CategoryName = "Biography", DisplayOrder = 3 },
            //new Category { Category_Id = 4, CategoryName = "Business", DisplayOrder = 4 },
            //new Category { Category_Id = 5, CategoryName = "Classic", DisplayOrder = 5 },
            //new Category { Category_Id = 6, CategoryName = "Drama", DisplayOrder = 6 },
            //new Category { Category_Id = 7, CategoryName = "Fantasy", DisplayOrder = 7 },
            //new Category { Category_Id = 8, CategoryName = "History", DisplayOrder = 8 },
            //new Category { Category_Id = 9, CategoryName = "Science", DisplayOrder = 9 },
            //new Category { Category_Id = 10, CategoryName = "Technology", DisplayOrder = 10 }
            //    );

            //            modelBuilder.Entity<Product>().HasData(
            //new Product
            //{
            //    Id = 1,
            //    Title = "Shadows of Fear",
            //    Author = "Ron Parker",
            //    Description = "Horror",
            //    ISBN = "FOT000000001",
            //    ListPrice = 45,
            //    Price = 40,
            //    Price100 = 35,
            //    Price50 = 38
            //    ,
            //    Category_Id = 9,
            //},

            //new Product
            //{
            //    Id = 2,
            //    Title = "The Silent Mind",
            //    Author = "Emily Stone",
            //    Description = "Psychological Thriller",
            //    ISBN = "FOT000000002",
            //    ListPrice = 30,
            //    Price = 28,
            //    Price100 = 24,
            //    Price50 = 26,
            //    Category_Id = 3,
            //},

            //new Product
            //{
            //    Id = 3,
            //    Title = "Code of Destiny",
            //    Author = "Michael Reeves",
            //    Description = "Science Fiction",
            //    ISBN = "FOT000000003",
            //    ListPrice = 50,
            //    Price = 47,
            //    Price100 = 42,
            //    Price50 = 45
            //    ,
            //    Category_Id = 5,
            //},

            //new Product
            //{
            //    Id = 4,
            //    Title = "Whispers in the Dark",
            //    Author = "Sophia Miller",
            //    Description = "Mystery",
            //    ISBN = "FOT000000004",
            //    ListPrice = 35,
            //    Price = 32,
            //    Price100 = 27,
            //    Price50 = 30,
            //    Category_Id = 7,
            //},

            //new Product
            //{
            //    Id = 5,
            //    Title = "Broken Kingdom",
            //    Author = "Liam Anderson",
            //    Description = "Fantasy",
            //    ISBN = "FOT000000005",
            //    ListPrice = 48,
            //    Price = 44,
            //    Price100 = 39,
            //    Price50 = 42,
            //    Category_Id = 1,
            //},

            //new Product
            //{
            //    Id = 6,
            //    Title = "Beyond the Horizon",
            //    Author = "Olivia Brown",
            //    Description = "Adventure",
            //    ISBN = "FOT000000006",
            //    ListPrice = 28,
            //    Price = 25,
            //    Price100 = 20,
            //    Price50 = 23,
            //    Category_Id = 7,
            //},

            //new Product
            //{
            //    Id = 7,
            //    Title = "Fragments of Truth",
            //    Author = "Daniel Cooper",
            //    Description = "Drama",
            //    ISBN = "FOT000000007",
            //    ListPrice = 40,
            //    Price = 36,
            //    Price100 = 31,
            //    Price50 = 34,
            //    Category_Id = 2,
            //}
            //);

        }
    }
}
