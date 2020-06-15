﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Model
{
    public class ClientRegistration : IClientRegistration
    {
        public ClientRegistration(string clientId, byte[] clientSecretHash, byte[] clientSecretSalt, ClientType clientType, TimeSpan tokenExpiration, IEnumerable<string> validRedirectUrls)
        {
            ClientId = clientId;
            ClientSecretHash = clientSecretHash;
            ClientSecretSalt = clientSecretSalt;
            ClientType = clientType;
            TokenExpiration = tokenExpiration;
            ValidRedirectUrls = validRedirectUrls.Select(p => new Uri(p)).ToImmutableList();
        }

        public string ClientId { get; }

        public byte[] ClientSecretHash { get; }

        public byte[] ClientSecretSalt { get; }

        public TimeSpan TokenExpiration { get; }

        public IReadOnlyList<Uri> ValidRedirectUrls { get; }

        public ClientType ClientType { get; }

        public IUser User => null;
    }
}