import { Component, input } from '@angular/core';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import * as Leaflet from 'leaflet';

export interface OsmMapMarker {
  content: string;
  lat: number;
  long: number;
}

@Component({
  selector: 'app-osm-map',
  standalone: true,
  imports: [
    LeafletModule
  ],
  templateUrl: './osm-map.component.html',
  styleUrl: './osm-map.component.scss'
})
export class OsmMapComponent {

  public width = input.required<string>();
  public height = input.required<string>();

  public centerLatLong = input<OsmMapMarker>();

  public markerLatLongs = input<OsmMapMarker[]>();

  onMapReady(map: Leaflet.Map) {

  }

  options(): Leaflet.MapOptions {
    return {
      layers: this.getLayers(),
      zoom: 16,
      center: this.centerLatLong() ? new Leaflet.LatLng(this.centerLatLong()!.lat, this.centerLatLong()!.long) : new Leaflet.LatLng(0, 0),
    }
  };

  getMarkers() {
    const markerPopup = new Leaflet.Popup();
    markerPopup.setContent(this.centerLatLong()?.content ?? '');

    return new Leaflet.Marker(this.centerLatLong() ? new Leaflet.LatLng(this.centerLatLong()!.lat, this.centerLatLong()!.long) : new Leaflet.LatLng(0, 0), {
        title: 'Workspace',
        icon: new Leaflet.Icon({
          iconSize: [35, 35],
          iconAnchor: [17.5, 35],
          iconUrl: 'assets/location_marker_red.png',
          popupAnchor: [0, -33],
        }),
      } as Leaflet.MarkerOptions)
      .bindPopup(markerPopup).openPopup();
  }

  getLayers() :Leaflet.Layer[] {
    return [
      new Leaflet.TileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
      }),
      this.getMarkers(),
    ];
  }
}
