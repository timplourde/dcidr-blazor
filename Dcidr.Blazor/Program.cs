using Blazored.LocalStorage;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Dcidr.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddSingleton(typeof(DcidrAppModel));
            builder.Services.AddBlazoredLocalStorage();
            
            builder.RootComponents.Add<App>("app");
            
            await builder.Build().RunAsync();
        }
    }
}
