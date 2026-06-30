using Connect4.Api.Services;
using Connect4.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RoomService>();
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSignalR();

var app = builder.Build();

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
