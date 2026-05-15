
using Bus.Shared.Consumer;
using Bus.Shared.Producer;
using Bus.Shared.Service;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UnadeskTest.PdfProcessor;
using UnadeskTest.Workers;
using UnadeskTest.BusinessLogic.Repositories;
using UnadeskTest.BusinessLogic.Services;
using DataAccess.Models;
using Bus.Shared.Configuration;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        var postgresConnectionString = hostContext.Configuration.GetConnectionString("PostgresConnection");
        services.AddDbContext<UnadeskDbContext>(options =>
        //Добавляем ENUM как тип в базу.
        options.UseNpgsql(postgresConnectionString, o => o.MapEnum<FileStatus>(nameof(FileStatus))));
        services.AddScoped<IPdfProcessor, PdfProcessor>();
        services.AddScoped<IMessageQueueConsumer, MessageQueueConsumer>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddHostedService<BgWorker>();
        //Для graceful shutdown
        services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromMinutes(5));

        services.Configure<RabbitConfiguration>(rabbitConfig => hostContext.Configuration.GetSection("Rabbit").Bind(rabbitConfig));
        services.AddScoped<IMessageQueueProducer, MessageQueueProducer>();
        services.AddSingleton<IRabbitMqService, RabbitMqService>();
    }).Build();

// Убедимся, что база данных готова (Миграции и создание таблиц)
// Вообще по хорошему это надо выносить в отдельный проект миграции, но в рамках тестового задания я не стал этого делать.

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UnadeskDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}


await app.RunAsync();
