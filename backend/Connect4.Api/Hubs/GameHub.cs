using Microsoft.AspNetCore.SignalR;
using Connect4.Api.Services;
using Connect4.Api.Models;

namespace Connect4.Api.Hubs;

public class GameHub : Hub
{
    private readonly RoomService _roomService;
    private readonly GameEngine _gameEngine;

    public GameHub(RoomService roomService, GameEngine gameEngine)
    {
        _roomService = roomService;
        _gameEngine = gameEngine;
    }

    public async Task JoinRoom(Guid roomId)
    {
        var room = _roomService.GetRoom(roomId);

        if (room is null)
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
            return;
        }

        JoinResult res = _gameEngine.Join(room, Context.ConnectionId);

        if(!res.Success){
            var msg = res.Error switch
            {
                JoinError.RoomFull => "Room is full",
                _ => "Error"
            };

            await Clients.Caller.SendAsync("Error", msg);
          return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        await Clients.Group(roomId.ToString()).SendAsync("PlayerJoined", room);
    }

    public async Task MakeMove(Guid roomId, int column)
    {
        var room = _roomService.GetRoom(roomId);
        if (room is null)
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
            return;
        }

        int playerNumber;
        if (Context.ConnectionId == room.Player1Id) playerNumber = 1;
        else if (Context.ConnectionId == room.Player2Id) playerNumber = 2;
        else
        {
            await Clients.Caller.SendAsync("Error", "Not a Player in this room");
            return;
        }

        MoveResult res = _gameEngine.MakeMove(room, playerNumber, column);

        if (!res.Success){
            var msg = res.Error switch
            {
                MoveError.GameNotActive => "Game is not Active",
                MoveError.NotYourTurn => "Its not your turn",
                MoveError.InvalidColumn => "Invalid column",
                MoveError.ColumnFull => "Column is full",
                _ => "Error"
            };
            await Clients.Caller.SendAsync("Error", msg);
            return;
        }

        await Clients.Group(roomId.ToString()).SendAsync("MoveMade", room);
    }


    public async Task RestartGame(Guid roomId)
    {
        var room = _roomService.GetRoom(roomId);
        if (room is null)
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
            return;
        }

        RestartResult res = _gameEngine.Restart(room, Context.ConnectionId);
        if(!res.Success){
            var msg = res.Error switch
            {
                RestartError.GameNotFinished => "Game is not finished",
                RestartError.NotAPlayer => "You are not a player in this room",
                _ => "Error"
            };

            await Clients.Caller.SendAsync("Error",msg);
            return;
        }

        await Clients.Group(roomId.ToString()).SendAsync("GameRestarted", room);
    }


    public override async Task OnDisconnectedAsync(Exception? exception){
        var room = _roomService.GetRoomByConnection(Context.ConnectionId);
        if(room is not null){
          if(room.Player1Id == Context.ConnectionId) room.Player1Id = null;
          else if(room.Player2Id == Context.ConnectionId) room.Player2Id = null;

          room.Reset();
          room.State = GameState.Waiting;
          await Clients.Group(room.Id.ToString()).SendAsync("PlayerLeft", room);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
