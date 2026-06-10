export const environment = {
  production: false,
  apiBaseUrl: '/api',
  /** Flip to true to develop without the FuelPriceWizard.API + LocalDB running. */
  useMockStations: false,
  /** The API does not expose price endpoints yet — keep mocking until they exist. */
  useMockPrices: true,
};
