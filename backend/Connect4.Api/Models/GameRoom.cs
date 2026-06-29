namespace Connect4.Api.Models;

public enum GameState
{
    Waiting,
    Active,
    Finished
}

public class GameRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public GameState State { get; set; }
    public string? Player1Id { get; set; }
    public string? Player2Id { get; set; }
    public int? Winner { get; set; }
    public int CurrentTurn { get; set; } = 1;
    public int[][] Board { get; set; } = Enumerable.Range(0, 6).Select(_ => new int[7]).ToArray();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public void Reset(){
        Board = Enumerable.Range(0,6).Select(_ => new int[7]).ToArray();
        CurrentTurn = 1;
        Winner = null;
    }

    public static bool CheckWin(int[][] board, int row, int col, int player)
    {
        int[][] directions ={
        new[] {0, 1},
        new[] {1, 1},
        new[] {1, 0},
        new[] {1, -1},
      };

        foreach (var dir in directions)
        {
            int dr = dir[0], dc = dir[1];
            int count = 1;

            for (int step = 1; step < 4; step++)
            {
                int r = row + dr * step;
                int c = col + dc * step;

                if (r < 0 || r >= 6 || c < 0 || c >= 7) break;
                if (board[r][c] != player) break;
                count++;
            }
            for (int step = 1; step < 4; step++)
            {
                int r = row - dr * step;
                int c = col - dc * step;

                if (r < 0 || r >= 6 || c < 0 || c >= 7) break;
                if (board[r][c] != player) break;
                count++;
            }

            if (count >= 4) return true;
        }
        return false;
    }

    public static bool IsBoardFull(int[][] board)
    {
        for (int c = 0; c < 7; c++)
        {
            if (board[0][c] == 0) return false;
        }
        return true;
    }
}
