import { Routes } from '@angular/router';
import { Home } from './features/home/home'
import { Game } from './features/game/game'

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'game/:id', component: Game }
];
