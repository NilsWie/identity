﻿using Codeworx.Identity.EntityFrameworkCore.Mappings;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class CodeworxIdentityDbContext : DbContext
    {
        public CodeworxIdentityDbContext(DbContextOptions<CodeworxIdentityDbContext> options)
            : base(options)
        {
        }

        protected CodeworxIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<IdentityCache> IdentityCaches { get; set; }

        public DbSet<RightHolderGroup> RightHolderGroups { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<TenantUser> TenantUsers { get; set; }

        public DbSet<ClientConfiguration> ClientConfigurations { get; set; }

        public DbSet<AuthenticationProvider> AuthenticationProviders { get; set; }

        public DbSet<AuthenticationProviderRightHolder> AuthenticationProviderRightHolders { get; set; }

        public DbSet<AvailableLicense> AvailableLicenses { get; set; }

        public DbSet<ClaimType> ClaimTypes { get; set; }

        public DbSet<UserInvitation> UserInvitations { get; set; }

        public DbSet<UserPasswordHistory> UserPasswordHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuthenticationProviderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationProviderRightHolderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AvailableLicenseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClaimTypeEntityTypeProvider());
            modelBuilder.ApplyConfiguration(new ClaimValueEntityTypeProvider());
            modelBuilder.ApplyConfiguration(new ClientConfigurationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientLicenseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientScopeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityCacheEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LicenseAssignmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderFilterEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RightHolderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RightHolderGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeAssignmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeClaimEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeHierarchyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserInvitationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserPasswordHistoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRefreshTokenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ValidRedirectUrlEntityTypeConfiguration());

            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}