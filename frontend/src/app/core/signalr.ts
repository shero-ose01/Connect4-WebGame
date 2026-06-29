import { Service, signal, computed } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { GameRoom } from '../models/game-room';

@Service()
export class GameService {
  private connection: signalR.HubConnection | null = null;

  private connectionId = signal<string | null>(null);

  room = signal<GameRoom | null>(null);
  error = signal<string | null>(null);
  playerNumber = computed(() => {
    const room = this.room();
    const id = this.connectionId();
    if (!room || !id) return null;
    if (room.player1Id === id) return 1;
    if (room.player2Id === id) return 2;
    return null;
  });

  isMyTurn = computed(() => {
    const room = this.room();
    const player = this.playerNumber();
    if (!room || !player) return false;
    return room.currentTurn === player;
  });

  async connect() {
    this.connection = new signalR.HubConnectionBuilder().withUrl('/gameHub').build();

    this.connection.on('PlayerJoined', (room: GameRoom) => {
      this.room.set(room);
    });

    this.connection.on('Error', (msg: string) => {
      this.error.set(msg);
    });

    this.connection.on('MoveMade', (room: GameRoom) => {
      this.room.set(room);
    });

    this.connection.on('GameRestarted', (room: GameRoom) => {
      this.room.set(room);
    });

    await this.connection.start();
    this.connectionId.set(this.connection.connectionId);
  }

  async joinRoom(roomId: string) {
    await this.connection?.invoke('JoinRoom', roomId);
  }

  async makeMove(roomId: string, column: number) {
    await this.connection?.invoke('MakeMove', roomId, column);
  }

  async restartGame(roomId: string) {
    await this.connection?.invoke('restartGame', roomId);
  }
}
