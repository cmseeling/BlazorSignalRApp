using BlazorSignalRApp.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SoloX.BlazorJsonLocalization;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient(
    "Host",
    (sp, c) =>
    {
        var hostEnv = sp.GetRequiredService<IWebAssemblyHostEnvironment>();
        c.BaseAddress = new Uri(hostEnv.BaseAddress);
    });

builder.Services.AddJsonLocalization(b =>
{
    b
        .AddFallback("Fallback", typeof(_Imports).Assembly)
        .UseHttpClientJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.NamingPolicy = HttpHostedJsonNamingPolicy;
            options.ApplicationAssembly = typeof(_Imports).Assembly;
            options.HttpClientBuilder = sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return httpClientFactory.CreateClient("Host");
            };
        });
});

builder.Services.AddLocalization();

var host = builder.Build();

const string defaultCulture = "en-US";

var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

if (result == null)
{
    await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();


static Uri HttpHostedJsonNamingPolicy(string basePath, string cultureName)
{
    return string.IsNullOrEmpty(cultureName)
        ? new Uri($"{basePath}.json?v=1.0.0", UriKind.Relative)
        : new Uri($"{basePath}.{cultureName}.json?v=1.0.0", UriKind.Relative);
}
