using Connect4.Api.Models;
using Connect4.Api.Services;

namespace Connect4.Tests;

public class GameEngineTests
{
    private readonly GameEngine _engine = new();

    private GameRoom ActiveRoom()
    {
        var room = new GameRoom();
        room.Player1Id = "pl1";
        room.Player2Id = "pl2";
        room.State = GameState.Active;
        return room;
    }

    [Fact]
    public void Join_FirstPlayer()
    {
        var room = new GameRoom();
        var result = _engine.Join(room, "pl1");

        Assert.True(result.Success);
        Assert.Equal(1, result.PlayerNumber);
        Assert.Equal("pl1", room.Player1Id);
    }

    [Fact]
    public void Join_SecondPlayer_ActivateGame()
    {
        var room = new GameRoom();
        _engine.Join(room, "pl1");
        var result = _engine.Join(room, "pl2");

        Assert.True(result.Success);
        Assert.Equal(2, result.PlayerNumber);
        Assert.Equal("pl2", room.Player2Id);
    }

    [Fact]
    public void Join_ThirdPlayer_ReturnsRoomFull()
    {
        var room = new GameRoom();
        _engine.Join(room, "pl1");
        _engine.Join(room, "pl2");
        var result = _engine.Join(room, "pl3");

        Assert.False(result.Success);
        Assert.Equal(JoinError.RoomFull, result.Error);
    }

    [Fact]
    public void MakeMove_ValidMove_SwitchesTurn()
    {
        var room = ActiveRoom();
        _engine.MakeMove(room, 1, 3);

        Assert.Equal(2, room.CurrentTurn);
    }

    [Fact]
    public void MakeMove_ColumnFull_ReturnsError()
    {
        var room = ActiveRoom();
        for (int i = 0; i < 6; i++)
            room.Board[i][0] = 1;

        var result = _engine.MakeMove(room, 1, 0);

        Assert.False(result.Success);
        Assert.Equal(MoveError.ColumnFull, result.Error);
    }

    [Fact]
    public void MakeMove_GameNotActive_ReturnsError()
    {
        var room = new GameRoom();
        var result = _engine.MakeMove(room, 1, 3);

        Assert.False(result.Success);
        Assert.Equal(MoveError.GameNotActive, result.Error);
    }

    [Fact]
    public void MakeMove_WrongTurn_ReturnsError()
    {
        var room = ActiveRoom();
        var result = _engine.MakeMove(room, 2, 3);

        Assert.False(result.Success);
        Assert.Equal(MoveError.NotYourTurn, result.Error);
    }

    [Fact]
    public void Restart_NotAPlayer_ReturnsError()
    {
        var room = ActiveRoom();
        room.State = GameState.Finished;
        var result = _engine.Restart(room, "notaplayer");

        Assert.False(result.Success);
        Assert.Equal(RestartError.NotAPlayer, result.Error);
    }

    [Fact]
    public void Restart_GameNotFinished_ReturnsError()
    {
        var room = ActiveRoom();
        var result = _engine.Restart(room, "pl1");

        Assert.False(result.Success);
        Assert.Equal(RestartError.GameNotFinished, result.Error);
    }

    [Fact]
    public void Restart_ValidRestart_ResetsBoard()
    {
        var room = ActiveRoom();
        room.State = GameState.Finished;
        room.Board[5][3] = 1;

        _engine.Restart(room, "pl1");

        Assert.Equal(GameState.Active, room.State);
        Assert.Null(room.Winner);
        Assert.Equal(0, room.Board[5][3]);
    }
}
