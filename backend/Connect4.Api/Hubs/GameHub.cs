using Microsoft.AspNetCore.SignalR;
using Connect4.Api.Services;
using Connect4.Api.Models;

namespace Connect4.Api.Hubs;

public class GameHub : Hub
{
    private readonly RoomService _roomService;

    public GameHub(RoomService roomService)
    {
        _roomService = roomService;
    }

    public async Task JoinRoom(Guid roomId)
    {
        var room = _roomService.GetRoom(roomId);

        if (room is null)
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
            return;
        }

        if (room.Player1Id is null)
        {
            room.Player1Id = Context.ConnectionId;
        }
        else if (room.Player2Id is null)
        {
            room.Player2Id = Context.ConnectionId;
            room.State = GameState.Active;
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Room is full");
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

        if (room.State != GameState.Active)
        {
            await Clients.Caller.SendAsync("Error", "Game not active");
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

        if (room.CurrentTurn != playerNumber)
        {
            await Clients.Caller.SendAsync("Error", "Not your turn");
            return;
        }

        if (column < 0 || column >= 7)
        {
            await Clients.Caller.SendAsync("Error", "Invalid column");
            return;
        }

        int targetRow = -1;

        for (int i = 5; i >= 0; i--)
        {
            if (room.Board[i][column] == 0)
            {
                targetRow = i;
                break;
            }
        }

        if (targetRow == -1)
        {
            await Clients.Caller.SendAsync("Error", "Column is full");
            return;
        }

        room.Board[targetRow][column] = playerNumber;

        if (GameRoom.CheckWin(room.Board, targetRow, column, playerNumber))
        {
            room.State = GameState.Finished;
            room.Winner = playerNumber;
        }
        else if (GameRoom.IsBoardFull(room.Board))
        {
            room.State = GameState.Finished; // this is a draw, no winner
        }
        else
        {
            room.CurrentTurn = playerNumber == 1 ? 2 : 1;
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

        if (Context.ConnectionId != room.Player1Id && Context.ConnectionId != room.Player2Id)
        {
            await Clients.Caller.SendAsync("Error", "You are not a Player in this room");
            return;
        }

        if (room.State != GameState.Finished)
        {
            await Clients.Caller.SendAsync("Error", "Game is still running");
            return;
        }

        room.Board = Enumerable.Range(0, 6).Select(_ => new int[7]).ToArray();
        room.CurrentTurn = 1;
        room.Winner = null;
        room.State = GameState.Active;

        await Clients.Group(roomId.ToString()).SendAsync("GameRestarted", room);
    }
}
