﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marvin.BlazorBFF.Client.Security
{
    //public class BffAuthenticationStateProvider1 : AuthenticationStateProvider
    //{
    //    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    //    }
    //}

    // thanks to Bernd Hirschmann for this code
    // https://github.com/berhir/BlazorWebAssemblyCookieAuth
    public class BffAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly TimeSpan UserCacheRefreshInterval = TimeSpan.FromSeconds(60);

        private readonly NavigationManager _navigation;
        private readonly HttpClient _client;
        private readonly ILogger<BffAuthenticationStateProvider> _logger;

        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        public BffAuthenticationStateProvider(NavigationManager navigation, HttpClient client,
            ILogger<BffAuthenticationStateProvider> logger)
        {
            _navigation = navigation;
            _client = client;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(await GetUser(useCache: true));
        }

        private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
        {
            var now = DateTimeOffset.Now;
            if (useCache && now < _userLastCheck + UserCacheRefreshInterval)
            {
                _logger.LogDebug("Taking user from cache");
                return _cachedUser;
            }

            _logger.LogDebug("Fetching user");
            _cachedUser = await FetchUser();
            _userLastCheck = now;

            return _cachedUser;
        }

        record ClaimRecord(string Type, string Value);

        private async Task<ClaimsPrincipal> FetchUser()
        {
            try
            {
                _logger.LogInformation("Fetching user information.");
                var response = await _client.GetAsync("bff/user");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var claims = await response.Content.ReadFromJsonAsync<List<ClaimRecord>>();

                    var identity = new ClaimsIdentity(
                        nameof(BffAuthenticationStateProvider),
                        "name",
                        "role");

                    foreach (var claim in claims)
                    {
                        identity.AddClaim(new Claim(claim.Type, claim.Value));
                    }

                    return new ClaimsPrincipal(identity);
                }
                else
                {
                    _logger.LogInformation($"Reponse {response.StatusCode}, {response.ReasonPhrase} received.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fetching user failed.");
            }

            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
