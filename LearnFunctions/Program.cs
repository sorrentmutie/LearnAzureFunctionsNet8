using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LearnFunctions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FunctionsDebugger.Enable();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(
                     configureDelegate => {
                         configureDelegate.AddHttpClient("RandomUserApi", client =>
                         {
                             client.BaseAddress = new System.Uri("https://randomuser.me/api");
                         });
                         configureDelegate.AddScoped<IClock, SystemClock>();
                         
                })
                .Build();

            host.Run();
        }
    }
}
