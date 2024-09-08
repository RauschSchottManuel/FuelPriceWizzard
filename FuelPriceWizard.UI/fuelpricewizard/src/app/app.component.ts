import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OsmMapComponent, OsmMapMarker } from "./shared/osm-map/osm-map.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, OsmMapComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  centerLatLong = {
    content: "Gas Station",
    lat: 48.31296055,
    long: 14.178722663837116
  } as OsmMapMarker
}
