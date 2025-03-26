using ManagementBot.Data;
using ManagementBot.Interfaces;
using ManagementBot.Service;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);


var botToken = builder.Configuration["TelegramBot:Token"];
builder.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));


builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite("Data Source=project_bot_db");
    options.EnableDetailedErrors();
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IBotService, BotService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var botService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IBotService>();
var cts = new CancellationTokenSource();
await botService.StartPollingAsync(cts.Token);

app.Run();
