using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookStoreApp.API.Data
{
    public partial class BookStoreDbContext : IdentityDbContext<ApiUser>
    {
        public BookStoreDbContext()
        {
        }

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Bio).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA09FAB742")
                    .IsUnique();

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.Isbn)
                    .HasMaxLength(50)
                    .HasColumnName("ISBN");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Summary).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Books_ToTable");
            });

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = "8343074e-8623-4e1a-b0c1-84fb8678c8f3",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                },
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = "c7ac6cfe-1f10-4baf-b604-cde350db9554",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
                }
            );

            var hasher = new PasswordHasher<ApiUser>();

            modelBuilder.Entity<ApiUser>().HasData(
                new ApiUser
                {
                    Id = "8e448afa-f008-446e-a52f-13c449803c2e",
                    Email = "admin@bookstore.com",
                    NormalizedEmail = "ADMIN@BOOKSTORE.COM",
                    UserName = "admin@bookstore.com",
                    NormalizedUserName = "ADMIN@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    //PasswordHash = hasher.HashPassword(null, "P@ssword1")
                    PasswordHash = "AQAAAAIAAYagAAAAEFeIDU62jVr9YhsqjKA9MzpmbjT8oDoK3J0N68wXacEpQZaGQw0+DL5jDVcN1bHY1A==",
                    ConcurrencyStamp = "9b2d2d02-1438-4b70-8e66-1b588b0bba01",     
                    SecurityStamp = "93d5a947-804f-48f1-a4fa-d8b780cfef63"
                },
                new ApiUser
                {
                    Id = "30a24107-d279-4e37-96fd-01af5b38cb27",
                    Email = "user@bookstore.com",
                    NormalizedEmail = "USER@BOOKSTORE.COM",
                    UserName = "user@bookstore.com",
                    NormalizedUserName = "USER@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "User",
                    //PasswordHash = hasher.HashPassword(null, "P@ssword1")
                    PasswordHash = "AQAAAAIAAYagAAAAEFbyfLBRT63ZvrT6Gj62Hq0Rt2yUVOCQKf/DzVtAi6zR+o64TTh4RWR2+rSseDEEJA==",
                    ConcurrencyStamp = "3d7addf1-a8ab-45a5-aadc-54640c116f22",
                    SecurityStamp = "439c89c2-04e2-47ef-a78f-b019c5367a2b"
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "8343074e-8623-4e1a-b0c1-84fb8678c8f3",
                    UserId = "30a24107-d279-4e37-96fd-01af5b38cb27"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "c7ac6cfe-1f10-4baf-b604-cde350db9554",
                    UserId = "8e448afa-f008-446e-a52f-13c449803c2e"
                }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}