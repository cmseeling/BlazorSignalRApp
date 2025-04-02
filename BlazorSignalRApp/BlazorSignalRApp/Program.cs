using BlazorSignalRApp.Components;
using Microsoft.AspNetCore.ResponseCompression;
using BlazorSignalRApp.Hubs;
using BlazorSignalRApp.Shared;
using SoloX.BlazorJsonLocalization.ServerSide;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServerSideJsonLocalization(builder =>
{
    builder
        .AddFallback("Fallback", typeof(BlazorSignalRApp.Client._Imports).Assembly)
        .UseHttpHostedJson(options =>
        {
            options.ResourcesPath = "Resources";
            options.ApplicationAssemblies = [typeof(_Imports).Assembly, typeof(BlazorSignalRApp.Client._Imports).Assembly];
        });
});

builder.Services.AddControllers(); //added for API

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddSingleton<Dictionary<string, LanguageQueue>>();

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorSignalRApp.Client._Imports).Assembly);

app.MapHub<ChatHub>("/chathub");
app.MapHub<MatchHub>("/matchhub");

app.Run();
