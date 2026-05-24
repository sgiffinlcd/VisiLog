using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VisiLog.Web.Blazor.WASM.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HttpClient pointed at the WASM host so the Abstraction.Service.WebAPI clients can call the local WebAPI.
// Registered here because BaseAddress comes from the WebAssembly host environment, which is unavailable inside Loader.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Loads dependency injection for the library and any child libraries that are subscribed to by this library. This is for the trunk library to load dependency injection for itself and any child libraries that are subscribed to by this library. Leaf to trunk load order.
var loader = new Loader();
loader.Load(builder.Services,builder.Configuration);

await builder.Build().RunAsync();
