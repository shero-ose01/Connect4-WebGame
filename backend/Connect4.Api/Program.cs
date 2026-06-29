using Connect4.Api.Services;
using Connect4.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<RoomService>();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/rooms", (RoomService roomService) =>
{
    var room = roomService.CreateRoom();
    return Results.Ok(room.Id);
});

app.MapGet("/api/rooms/{id}", (Guid id, RoomService roomService) =>
{
    var room = roomService.GetRoom(id);
    return room is null ? Results.NotFound() : Results.Ok(room);
});

app.MapHub<GameHub>("/gameHub");

app.Run();
