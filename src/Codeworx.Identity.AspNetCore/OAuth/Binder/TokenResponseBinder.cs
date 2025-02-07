﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class TokenResponseBinder : ResponseBinder<TokenResponse>
    {
        private static readonly JsonSerializerSettings _serializerSettings;

        static TokenResponseBinder()
        {
            _serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public override async Task BindAsync(TokenResponse responseData, HttpResponse response)
        {
            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.Headers.Add(HeaderNames.ContentType, "application/json;charset=utf-8");
            response.Headers.Add(HeaderNames.CacheControl, "no-store");
            response.Headers.Add(HeaderNames.Pragma, "no-cache");

            var serializer = JsonSerializer.Create(_serializerSettings);

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 4096, true))
                {
                    serializer.Serialize(writer, responseData);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(response.Body).ConfigureAwait(false);
            }
        }
    }
}