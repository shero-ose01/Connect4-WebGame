import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GameService } from '../../core/signalr';

@Component({
  selector: 'app-game',
  imports: [],
  templateUrl: './game.html',
  styleUrl: './game.scss',
})
export class Game implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private gameService = inject(GameService);

  room = this.gameService.room;
  error = this.gameService.error;
  playerNumber = this.gameService.playerNumber;
  isMyTurn = this.gameService.isMyTurn;
  shareUrl = this.gameService.shareUrl;
  private roomId!: string;

  async ngOnInit() {
    this.roomId = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.roomId)
      return;

    await this.gameService.connect();
    await this.gameService.joinRoom(this.roomId);
  }

  async ngOnDestroy() {
    await this.gameService.disconnect();
  }

  dropPiece(column: number) {
    this.gameService.makeMove(this.roomId, column);
  }

  restart() {
    this.gameService.restartGame(this.roomId);
  }
}
