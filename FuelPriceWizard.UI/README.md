# FuelPriceWizard - UI

Angular 18 frontend that displays gas stations and their fuel prices on an interactive map. Built with TailwindCSS for styling and [ngx-leaflet](https://github.com/bluehalo/ngx-leaflet) (OpenStreetMap) for the map view.

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 18 | Frontend framework |
| TailwindCSS | 3 | Utility-first CSS styling |
| ngx-leaflet | 18 | OpenStreetMap integration |
| RxJS | 7 | Reactive data streams |
| Karma + Jasmine | — | Unit testing |
| ESLint | — | Linting |

## Quick Start

The Angular application lives in the `fuelpricewizard/` subdirectory.

```bash
cd fuelpricewizard
npm ci
ng serve
```

Navigate to `http://localhost:4200/`. The app reloads automatically on source file changes.

## Available Scripts

| Command | Description |
|---------|-------------|
| `ng serve` | Start the development server |
| `ng build` | Production build (output to `dist/`) |
| `ng test` | Run unit tests with coverage via Karma |
| `ng lint` | Run ESLint |

See the [Angular app README](fuelpricewizard/README.md) for full Angular CLI command reference.
