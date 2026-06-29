import { Service } from '@angular/core';
import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Service()
export class Room {

  private http = inject(HttpClient);

  createRoom() {
    return this.http.post<string>('/api/rooms', {});
  }
}

