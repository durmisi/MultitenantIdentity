using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MultitenantHybridClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:8000", "http://*:8001", "http://*:8002", "http://*:8003", "http://*:8004")
                .Build();
    }
}
