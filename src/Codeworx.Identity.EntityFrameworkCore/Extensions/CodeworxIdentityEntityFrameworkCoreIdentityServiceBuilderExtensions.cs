﻿using System;
using Codeworx.Identity.Account;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.EntityFrameworkCore.Account;
using Codeworx.Identity.EntityFrameworkCore.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseDbContext(this IIdentityServiceBuilder builder, Action<IServiceProvider, DbContextOptionsBuilder> contextBuilder)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(contextBuilder);

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContext(this IIdentityServiceBuilder builder, Action<DbContextOptionsBuilder> contextBuilder)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(contextBuilder);

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContext<TContext>(this IIdentityServiceBuilder builder)
            where TContext : DbContext
        {
            var result = builder
                         .Users<EntityUserService<TContext>>()
                         .LoginRegistrations<LoginRegistrationProvider<TContext>>()
                         .Tenants<EntityTenantService<TContext>>()
                         .Clients<EntityClientService<TContext>>()
                         .ReplaceService<IConfirmationService, EntityConfirmationService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IChangeUsernameService, EntityChangeUsernameService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IChangePasswordService, EntityChangePasswordService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IDefaultTenantService, EntityUserService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IFailedLoginService, EntityUserService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<ILinkUserService, EntityUserService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IAuthorizationCodeCache, AuthorizationCodeCache<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IRefreshTokenCache, RefreshTokenCache<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IStateLookupCache, StateLookupCache<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IInvitationCache, InvitationCache<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IExternalTokenCache, ExternalTokenCache<TContext>>(ServiceLifetime.Scoped)
                         .RegisterMultiple<IScopeProvider, EntityScopeProvider<TContext>>(ServiceLifetime.Scoped)
                         .RegisterMultiple<ISystemScopeProvider, SystemScopeProvider>(ServiceLifetime.Scoped)
                         .RegisterMultiple<ISystemClaimsProvider, SystemClaimsProvider<TContext>>(ServiceLifetime.Transient);

            return result;
        }
    }
}