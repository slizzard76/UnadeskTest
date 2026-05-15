using Bus.Shared.Configuration;
using Bus.Shared.Producer;
using Bus.Shared.Service;
using DataAccess.Context;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using UnadeskTest.BusinessLogic.Repositories;
using UnadeskTest.BusinessLogic.Services;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<UnadeskDbContext>(options =>
     options.UseNpgsql(postgresConnectionString, o => o.MapEnum<FileStatus>(nameof(FileStatus))));

builder.Services.AddScoped<IMessageQueueProducer, MessageQueueProducer>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.Configure<RabbitConfiguration>(rabbitConfiguration => builder.Configuration.GetSection("Rabbit").Bind(rabbitConfiguration));

builder.Services.AddScoped<IMessageQueueProducer, MessageQueueProducer>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UnadeskTest API",
        Version = "v1",
        Description = "API для теста."
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UnadeskTest API");

    c.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
