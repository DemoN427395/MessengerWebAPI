using Microsoft.AspNetCore;

namespace GATEAWAY;

public class Gateway
{
    public static async Task Main(string[] args)
    {
        var builder = WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>()
            .UseUrls("http://localhost:4000");
        var host = builder.Build();
        await host.RunAsync();
    }
}