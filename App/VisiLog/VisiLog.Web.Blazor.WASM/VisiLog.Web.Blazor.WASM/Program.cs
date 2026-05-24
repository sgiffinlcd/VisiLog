using VisiLog.Model.Service.WebAPI;
using VisiLog.Service.WebAPI;
using VisiLog.Web.Blazor.WASM;
using VisiLog.Web.Blazor.WASM.Client.Pages;
using VisiLog.Web.Blazor.WASM.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Using OpenAPI
builder.Services.AddOpenApi();

// Activate System.Text.Json source generation for the WebAPI's response DTOs.
// Inserting at index 0 makes the source-gen context take precedence over the default reflection resolver.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, VisiLogJsonSerializerContext.Default));

// Dependency injection loading for the library and any child libraries that are subscribed to by this library. This is for the trunk library to load dependency injection for itself and any child libraries that are subscribed to by this library. Leaf to trunk load order.
var loader = new VisiLog.Web.Blazor.WASM.Loader();
loader.Load(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapVisiLogEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(VisiLog.Web.Blazor.WASM.Client._Imports).Assembly);

app.Run();
