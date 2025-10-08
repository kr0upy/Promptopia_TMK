using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TMK.Backend.Services;
using TMK.Backend.Data;

namespace TMK.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Запуск ASP.NET Core Web API
            var host = CreateHostBuilder(args).Build();

            // Запуск Telegram-бота в отдельном потоке
            var bot = new TmkBot();
            _ = bot.StartAsync(CancellationToken.None);

            // Импорт данных при первом запуске
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var importService = new DataImportService(db, "TMK_FILES"); // путь к папке с json
                await importService.ImportAllAsync();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
