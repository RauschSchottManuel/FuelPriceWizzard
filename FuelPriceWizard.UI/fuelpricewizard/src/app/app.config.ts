import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { GasStationDataSource } from './core/services/gas-station.data-source';
import { HttpGasStationDataSource } from './core/services/http-gas-station.data-source';
import { MockGasStationDataSource } from './core/services/mock-gas-station.data-source';
import { PriceDataSource } from './core/services/price.data-source';
import { HttpPriceDataSource } from './core/services/http-price.data-source';
import { MockPriceDataSource } from './core/services/mock-price.data-source';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch()),
    provideCharts(withDefaultRegisterables()),
    {
      provide: GasStationDataSource,
      useClass: environment.useMockStations ? MockGasStationDataSource : HttpGasStationDataSource,
    },
    {
      provide: PriceDataSource,
      useClass: environment.useMockPrices ? MockPriceDataSource : HttpPriceDataSource,
    },
  ],
};
