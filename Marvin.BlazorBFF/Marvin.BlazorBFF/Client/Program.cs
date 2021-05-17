using Marvin.BlazorBFF.Client.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.BlazorBFF.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.TryAddSingleton<AuthenticationStateProvider,
                BffAuthenticationStateProvider>();
            builder.Services.TryAddSingleton(sp =>
                (BffAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

            builder.Services.AddTransient<AntiforgeryHandler>();
            builder.Services.AddHttpClient("backend", client => client.BaseAddress =
                new Uri(builder.HostEnvironment.BaseAddress))
             .AddHttpMessageHandler<AntiforgeryHandler>();

            builder.Services.AddTransient(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));

            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddScoped(sp => new HttpClient
            //{ BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }


    }
}
