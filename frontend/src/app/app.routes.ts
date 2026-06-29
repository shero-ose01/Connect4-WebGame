import { Routes } from '@angular/router';
import { Home } from './features/home/home'
import { Game } from './features/game/game'
import { Layout } from './layout/layout'
export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      { path: '', component: Home },
      { path: 'game/:id', component: Game }
    ]
  }
];
