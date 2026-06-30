using Connect4.Api.Models;

namespace Connect4.Api.Services;

public enum MoveError    { None, GameNotActive, NotYourTurn, InvalidColumn, ColumnFull }
public enum JoinError    { None, RoomFull }
public enum RestartError { None, NotAPlayer, GameNotFinished }

public record MoveResult(bool Success, MoveError Error = MoveError.None);
public record JoinResult(bool Success, int PlayerNumber = 0, JoinError Error = JoinError.None);
public record RestartResult(bool Success, RestartError Error = RestartError.None);

public class GameEngine
{
    public JoinResult    Join(GameRoom room, string connectionId){

      if (room.Player1Id is null)
      {
          room.Player1Id = connectionId;
            return new JoinResult(true, 1);
        }
      else if (room.Player2Id is null)
      {
          room.Player2Id = connectionId;
          room.State = GameState.Active;
            return new JoinResult(true, 2);
        }
      else
      {
          return new JoinResult(false, 0, JoinError.RoomFull);
      }
    }

    public MoveResult    MakeMove(GameRoom room, int playerNumber, int column){

      if(room.State != GameState.Active){
          return new MoveResult(false, MoveError.GameNotActive);
      }
      if (room.CurrentTurn != playerNumber)
      {
          return new MoveResult(false, MoveError.NotYourTurn);
      }

      if (column < 0 || column >= 7)
      {
          return new MoveResult(false, MoveError.InvalidColumn);
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
          return new MoveResult(false, MoveError.ColumnFull);
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

      return new MoveResult(true);
    }

    public RestartResult Restart(GameRoom room, string connectionId){
      if (connectionId != room.Player1Id && connectionId != room.Player2Id)
      {
          return new RestartResult(false, RestartError.NotAPlayer);
      }

      if (room.State != GameState.Finished)
      {
          return new RestartResult(false, RestartError.GameNotFinished);
      }

      room.Reset();
      room.State = GameState.Active;
      return new RestartResult(true);
    }
}
