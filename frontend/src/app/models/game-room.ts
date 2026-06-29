export enum GameState {
  Waiting = 0,
  Active = 1,
  Finished = 2
}

export interface GameRoom {
  id: string;
  state: GameState;
  player1Id: string | null;
  player2Id: string | null;
  winner: number | null;
  currentTurn: number;
  board: number[][];
  createdAt: string;
}
