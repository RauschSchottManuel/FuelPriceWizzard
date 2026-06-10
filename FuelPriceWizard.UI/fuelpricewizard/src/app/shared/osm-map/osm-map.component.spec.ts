import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmMapComponent, OsmMapMarker } from './osm-map.component';
import { By } from '@angular/platform-browser';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { ComponentRef } from '@angular/core';
import { TileLayer } from 'leaflet';

describe('OsmMapComponent', () => {
  let component: OsmMapComponent;
  let fixture: ComponentFixture<OsmMapComponent>;
  let componentRef: ComponentRef<OsmMapComponent>;

  const markers: OsmMapMarker[] = [
    { id: 1, content: 'Station A', lat: 48.29, long: 14.29 },
    { id: 2, content: 'Station B', lat: 48.31, long: 14.28 },
    { id: 3, content: 'Station C', lat: 48.26, long: 14.31 },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeafletModule, OsmMapComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OsmMapComponent);
    component = fixture.componentInstance;
    componentRef = fixture.componentRef;
    componentRef.setInput('width', '500px');
    componentRef.setInput('height', '400px');
  });

  it('should create the OsmMapComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should set the correct width and height styles', () => {
    fixture.detectChanges();

    const mapDiv = fixture.debugElement.query(By.css('div[style]')).nativeElement;
    expect(mapDiv.style.width).toBe('500px');
    expect(mapDiv.style.height).toBe('400px');
  });

  it('should configure Leaflet map options from the center marker and zoom', () => {
    componentRef.setInput('centerLatLong', markers[0]);
    componentRef.setInput('zoom', 13);

    const options = component.mapOptions();
    expect(options.zoom).toBe(13);
    if (options.center instanceof Array) {
      expect(options.center[0]).toBe(markers[0].lat);
      expect(options.center[1]).toBe(markers[0].long);
    } else {
      expect(options.center?.lat).toBe(markers[0].lat);
      expect(options.center?.lng).toBe(markers[0].long);
    }
    expect(options.layers?.length).toBe(1);
    expect(options.layers?.[0]).toBeInstanceOf(TileLayer);
  });

  it('should create one marker layer per markerLatLongs entry', () => {
    componentRef.setInput('markerLatLongs', markers);

    const layers = component.markerLayers();
    expect(layers.length).toBe(markers.length);
    expect(layers[1].getLatLng().lat).toBe(markers[1].lat);
    expect(layers[1].getLatLng().lng).toBe(markers[1].long);
    expect(layers[1].getPopup()?.getContent()).toBe('Station B');
  });

  it('should fall back to the center marker when markerLatLongs is empty', () => {
    componentRef.setInput('centerLatLong', markers[0]);
    componentRef.setInput('markerLatLongs', []);

    const layers = component.markerLayers();
    expect(layers.length).toBe(1);
    expect(layers[0].getPopup()?.getContent()).toBe('Station A');
  });

  it('should update marker layers when the input changes', () => {
    componentRef.setInput('markerLatLongs', markers.slice(0, 1));
    expect(component.markerLayers().length).toBe(1);

    componentRef.setInput('markerLatLongs', markers);
    expect(component.markerLayers().length).toBe(3);
  });

  it('should compute bounds covering all markers (2+ markers only)', () => {
    componentRef.setInput('markerLatLongs', markers.slice(0, 1));
    expect(component.fitBounds()).toBeUndefined();

    componentRef.setInput('markerLatLongs', markers);
    const bounds = component.fitBounds();
    expect(bounds).toBeDefined();
    for (const marker of markers) {
      expect(bounds!.contains([marker.lat, marker.long])).toBeTrue();
    }
  });

  it('should emit markerClicked when a marker is clicked', () => {
    componentRef.setInput('markerLatLongs', markers);
    const clicked: OsmMapMarker[] = [];
    component.markerClicked.subscribe((marker: OsmMapMarker) => clicked.push(marker));

    component.markerLayers()[2].fire('click');

    expect(clicked.length).toBe(1);
    expect(clicked[0].id).toBe(3);
  });
});
