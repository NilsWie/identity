﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public class IdentityServiceBuilder : IIdentityServiceBuilder
    {
        private readonly IServiceCollection _collection;
        private readonly HashSet<Assembly> _parts;
        private bool _windowsAuthentication;

        public IdentityServiceBuilder(IServiceCollection collection)
        {
            _collection = collection;
            _parts = new HashSet<Assembly>();
            _windowsAuthentication = true;
            OptionsDelegate = p => { };

            _collection.AddScoped<IProviderSetup, EmptyProviderSetup>();
            _collection.AddScoped<IIdentityService, Identity.IdentityService>();
            _collection.AddScoped<IScopeService, DummyScopeService>();
        }

        public Action<IdentityOptions> OptionsDelegate { get; private set; }

        public IServiceCollection ServiceCollection => _collection;

        public IIdentityServiceBuilder AddPart(Assembly assembly)
        {
            _parts.Add(assembly);
            return this;
        }

        public IIdentityServiceBuilder Options(Action<IdentityOptions> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            OptionsDelegate = action;

            return this;
        }

        public IIdentityServiceBuilder PasswordValidator<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IPasswordValidator
        {
            RegisterScoped<IPasswordValidator, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder Provider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IProviderSetup
        {
            RegisterScoped<IProviderSetup, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder ReplaceService<TService, TImplementation>(ServiceLifetime lifeTime, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            Register<TService, TImplementation>(factory, lifeTime);
            return this;
        }

        public IdentityService ToService(IdentityOptions options, IEnumerable<IContentTypeProvider> contentTypeProviders = null)
        {
            return new IdentityService(options, _parts, contentTypeProviders, _windowsAuthentication);
        }

        public IIdentityServiceBuilder UserProvider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IUserService
        {
            RegisterScoped<IUserService, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder TenantProvider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, ITenantService
        {
            RegisterScoped<ITenantService, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder DefaultTenantProvider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IDefaultTenantService
        {
            RegisterScoped<IDefaultTenantService, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder View<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IViewTemplate
        {
            RegisterSingleton<IViewTemplate, TImplementation>(factory);
            return this;
        }

        public IIdentityServiceBuilder WindowsAuthentication(bool enable)
        {
            _windowsAuthentication = enable;
            return this;
        }

        private void Register<TService, TImplementation>(Func<IServiceProvider, TImplementation> factory, ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            var config = _collection.FirstOrDefault(p => p.ServiceType == typeof(TService));

            if (config != null)
            {
                _collection.Remove(config);
            }

            if (factory == null)
            {
                config = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
            }
            else
            {
                config = new ServiceDescriptor(typeof(TService), factory, lifetime);
            }

            _collection.Add(config);
        }

        private void RegisterScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            Register<TService, TImplementation>(factory, ServiceLifetime.Scoped);
        }

        private void RegisterSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            Register<TService, TImplementation>(factory, ServiceLifetime.Singleton);
        }

        private class DummyScopeService : IScopeService
        {
            public Task<IEnumerable<IScope>> GetScopes()
            {
                return Task.FromResult<IEnumerable<IScope>>(new List<IScope>
                                                            {
                                                                new DummyScope()
                                                            });
            }

            private class DummyScope : IScope
            {
                public string ScopeKey => Constants.DefaultScopeKey;
            }
        }

        private class EmptyProviderSetup : IProviderSetup
        {
            public Task<IEnumerable<ExternalProvider>> GetProvidersAsync(string userName = null)
            {
                return Task.FromResult(Enumerable.Empty<ExternalProvider>());
            }

            public Task<IUser> GetUserIdentity(string providerId, string nameIdentifier)
            {
                return Task.FromResult<IUser>(null);
            }
        }
    }
}