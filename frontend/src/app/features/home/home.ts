import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Room } from '../../core/room';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  private roomService = inject(Room);
  private router = inject(Router);

  createRoom() {
    this.roomService.createRoom().subscribe(roomId => {
      this.router.navigate(['/game', roomId]);
    });
  }
}
