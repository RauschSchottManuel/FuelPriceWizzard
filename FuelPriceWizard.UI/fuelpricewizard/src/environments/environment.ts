export const environment = {
  production: true,
  apiBaseUrl: '/api',
  /** Serve gas stations from the in-memory mock instead of the FuelPriceWizard.API. */
  useMockStations: false,
  /** The API does not expose price endpoints yet — keep mocking until they exist. */
  useMockPrices: true,
};
