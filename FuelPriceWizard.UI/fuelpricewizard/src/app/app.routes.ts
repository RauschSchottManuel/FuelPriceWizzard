import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'stations' },
  {
    path: 'stations',
    loadComponent: () =>
      import('./features/stations/stations-page.component').then((m) => m.StationsPageComponent),
  },
  {
    path: 'stations/:id',
    loadComponent: () =>
      import('./features/stations/station-detail/station-detail.component').then(
        (m) => m.StationDetailComponent,
      ),
  },
  { path: '**', redirectTo: 'stations' },
];
