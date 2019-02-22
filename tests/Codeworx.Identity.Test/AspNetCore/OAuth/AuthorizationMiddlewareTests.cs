﻿using System;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationMiddlewareTests : IntegrationTestBase
    {
        [Fact]
        public async Task Invoke_RedirectUriMissing_InformUserNoRedirect()
        {
            var emptyUri = string.Empty;
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={emptyUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Invoke_RedirectUriInvalid_InformUserNoRedirect()
        {
            const string InvalidUri = "x:invalidUri";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={InvalidUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Invoke_RedirectUriRelative_InformUserNoRedirect()
        {
            var invalidRedirect = "/invalidRedirect";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={invalidRedirect}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, responseHtml);
        }

        [Fact]
        public async Task Invoke_ClientIdentifierMissing_InformUserNoRedirect()
        {
            var emptyClientId = string.Empty;
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={emptyClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task Invoke_ClientIdentifierInvalid_InformUserNoRedirect()
        {
            var invalidClientId = "\u0019";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={invalidClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
            Assert.Contains(Identity.OAuth.Constants.ClientIdName, responseHtml);
        }

        [Fact]
        public async Task Invoke_EmptyQuery_InformUserNoRedirect()
        {
            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains(Identity.OAuth.Constants.Error.InvalidRequest, responseHtml);
        }

        [Fact]
        public async Task Invoke_RequiredParameterMissing_RedirectWithError()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidRequest}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.ResponseTypeName}", queryParts[1]);
        }

        [Fact]
        public async Task Invoke_InvalidParameter_RedirectWithError()
        {
            const string InvalidState = "ä";
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}&{Identity.OAuth.Constants.StateName}={InvalidState}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(3, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidRequest}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.StateName}", queryParts[1]);
            Assert.Equal($"{Identity.OAuth.Constants.StateName}={InvalidState}", queryParts[2]);
        }

        [Fact]
        public async Task Invoke_ClientNotAuthorized_RedirectWithError()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}&{Identity.OAuth.Constants.StateName}={request.State}";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(request.RedirectUri, $"{response.Headers.Location.Scheme}://{response.Headers.Location.Host}{response.Headers.Location.LocalPath}");

            var queryParts = response.Headers.Location.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.UnauthorizedClient}", queryParts[0]);
        }

        [Fact]
        public void Invoke_AccessDenied_RedirectWithError()
        {
            // ToDo: The resource owner or authorization server denied the request.
            throw new NotImplementedException();
        }

        [Fact]
        public void Invoke_UnsupportedResponseType_RedirectWithError()
        {
            // ToDo: The authorization server does not support obtaining an authorization code using this method.
            throw new NotImplementedException();
        }

        [Fact]
        public void Invoke_ScopeUnknown_RedirectWithError()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Invoke_ScopeWithInvalidCharacter_RedirectWithError()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var requestString = $"?{Identity.OAuth.Constants.ClientIdName}={request.ClientId}&{Identity.OAuth.Constants.RedirectUriName}={request.RedirectUri}&{Identity.OAuth.Constants.ResponseTypeName}={request.ResponseType}&{Identity.OAuth.Constants.ScopeName}=ä";

            var options = this.TestServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();
            var response = await this.TestClient.GetAsync(options.Value.OauthEndpoint + requestString);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(new Uri($"{request.RedirectUri}?{Identity.OAuth.Constants.ErrorName}={Identity.OAuth.Constants.Error.InvalidScope}&{Identity.OAuth.Constants.ErrorDescriptionName}={Identity.OAuth.Constants.ScopeName}"), response.Headers.Location);
        }

        [Fact]
        public void Invoke_ServerError_RedirectWithError()
        {
            // ToDo: The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
            // (This error code is needed because a 500 Internal Server Error HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }

        [Fact]
        public void Invoke_ServerTemporarilyUnavailable_RedirectWithError()
        {
            // ToDo: The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of the server.
            // (This error code is needed because a 503 Service Unavailable HTTP status code cannot be returned to the client via an HTTP redirect.)
            throw new NotImplementedException();
        }
    }
}
