﻿using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class PasswordChangeResponseBinder : ResponseBinder<PasswordChangeResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityAuthenticationHandler _handler;

        public PasswordChangeResponseBinder(
            IOptionsSnapshot<IdentityOptions> options,
            IBaseUriAccessor baseUriAccessor,
            IIdentityAuthenticationHandler handler)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
            _handler = handler;
        }

        public override async Task BindAsync(PasswordChangeResponse responseData, HttpResponse response)
        {
            var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login");

            if (!string.IsNullOrWhiteSpace(responseData.Prompt))
            {
                builder.AppendQueryParameter(Constants.OAuth.PromptName, responseData.Prompt);
            }

            if (!string.IsNullOrWhiteSpace(responseData.ReturnUrl))
            {
                builder.AppendQueryParameter(Constants.ReturnUrlParameter, responseData.ReturnUrl);
            }

            await _handler.SignOutAsync(response.HttpContext);

            response.Redirect(builder.ToString());
        }
    }
}
