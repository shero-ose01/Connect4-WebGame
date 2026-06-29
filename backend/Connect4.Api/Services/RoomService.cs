using Connect4.Api.Models;
using System.Collections.Concurrent;

namespace Connect4.Api.Services;

public class RoomService()
{
    private readonly ConcurrentDictionary<Guid, GameRoom> _rooms = new();

    public GameRoom CreateRoom()
    {
        GameRoom newRoom = new GameRoom();
        _rooms.TryAdd(newRoom.Id, newRoom);
        return newRoom;
    }
    public GameRoom? GetRoom(Guid id)
    {
        _rooms.TryGetValue(id, out var room);
        return room;
    }
}
