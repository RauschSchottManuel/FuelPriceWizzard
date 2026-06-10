import { Component, computed, input, output } from '@angular/core';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import * as Leaflet from 'leaflet';

export interface OsmMapMarker {
  /** Optional reference back to the entity the marker represents. */
  id?: number;
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

  public zoom = input<number>(16);

  public centerLatLong = input<OsmMapMarker>();

  public markerLatLongs = input<OsmMapMarker[]>();

  public markerClicked = output<OsmMapMarker>();

  /** Initial map options; ngx-leaflet only reads these once at map creation. */
  public mapOptions = computed<Leaflet.MapOptions>(() => {
    const center = this.centerLatLong() ?? this.markerLatLongs()?.[0];
    return {
      layers: [
        new Leaflet.TileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&copy; OpenStreetMap contributors'
        }),
      ],
      zoom: this.zoom(),
      center: center ? new Leaflet.LatLng(center.lat, center.long) : new Leaflet.LatLng(0, 0),
    };
  });

  /** One marker per markerLatLongs entry; falls back to the single center marker. */
  public markerLayers = computed<Leaflet.Marker[]>(() =>
    this.effectiveMarkers().map((marker) => this.createMarker(marker)),
  );

  /** Zoom the map so all markers are visible (only meaningful for 2+ markers). */
  public fitBounds = computed<Leaflet.LatLngBounds | undefined>(() => {
    const markers = this.effectiveMarkers();
    if (markers.length < 2) {
      return undefined;
    }
    return Leaflet.latLngBounds(markers.map((m) => new Leaflet.LatLng(m.lat, m.long)));
  });

  private effectiveMarkers = computed<OsmMapMarker[]>(() => {
    const markers = this.markerLatLongs();
    if (markers?.length) {
      return markers;
    }
    const center = this.centerLatLong();
    return center ? [center] : [];
  });

  private createMarker(marker: OsmMapMarker): Leaflet.Marker {
    const markerPopup = new Leaflet.Popup();
    markerPopup.setContent(marker.content);

    return new Leaflet.Marker(new Leaflet.LatLng(marker.lat, marker.long), {
        title: marker.content,
        icon: new Leaflet.Icon({
          iconSize: [35, 35],
          iconAnchor: [17.5, 35],
          iconUrl: 'assets/location_marker_red.png',
          popupAnchor: [0, -33],
        }),
      } as Leaflet.MarkerOptions)
      .bindPopup(markerPopup)
      .on('click', () => this.markerClicked.emit(marker));
  }
}
