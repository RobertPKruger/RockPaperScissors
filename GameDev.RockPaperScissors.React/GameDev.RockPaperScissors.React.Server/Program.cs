using GameDev.RockPaperScissors.GameAPI.ViewServices;
using GameDev.RockPaperScissors.React.Server.Websockets;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddHttpClient(); // Register IHttpClientFactory
builder.Services.AddScoped<INoaaAPI, NoaaAPI>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await RockPaperScissorsWebSockets.Echo(context, webSocket);
    }
    else
    {
        await next();
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
