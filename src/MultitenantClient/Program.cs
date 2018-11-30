using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MultitenantClient
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
                .UseUrls("http://*:7000", "http://*:7001", "http://*:7002", "http://*:7003", "http://*:7004")
                .Build();
    }
}
