using System;
using System.Collections.Generic;
using System.Text;
using AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Infrastructure.Data.Identity
{
    public class CommonDBContext<TUser, TRole> : IdentityDbContext<TUser, TRole, Guid>
        where TUser : User
        where TRole : Role
    {
        private readonly DbContextOptions _options;

        public CommonDBContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TUser>().ToTable("Users");
            modelBuilder.Entity<TRole>().ToTable("Roles"); 
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
        }
    }

    public class AppDBContext : CommonDBContext<User, Role>
    {
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
    }
}
