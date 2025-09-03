using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Poddle.CommunicationService.Configurations;
using Poddle.CommunicationService.Data;
using Poddle.CommunicationService.Middlewares;
using Poddle.CommunicationService.Repositories.Implementations;
using Poddle.CommunicationService.Repositories.Interfaces;
using Poddle.CommunicationService.Services.Implementations;
using Poddle.CommunicationService.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Poddle Communication Service API", Version = "v1" });
});

builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
builder.Services.Configure<AiChatbotSettings>(builder.Configuration.GetSection("AiChatbotSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<ISupportService, SupportService>();

builder.Services.AddHttpClient("WhatsApp", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<WhatsAppSettings>>().Value;
    if (Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var uri))
    {
        client.BaseAddress = uri;
    }
    if (!string.IsNullOrWhiteSpace(settings.ApiKey))
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {settings.ApiKey}");
    }
});

builder.Services.AddHttpClient("AiChatbot", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<AiChatbotSettings>>().Value;
    if (Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var uri))
    {
        client.BaseAddress = uri;
    }
    if (!string.IsNullOrWhiteSpace(settings.ApiKey))
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {settings.ApiKey}");
    }
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
