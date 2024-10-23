import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmMapComponent, OsmMapMarker } from './osm-map.component';
import { By } from '@angular/platform-browser';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { ComponentRef } from '@angular/core';
import { TileLayer, Marker } from 'leaflet';

describe('OsmMapComponent', () => {
  let component: OsmMapComponent;
  let fixture: ComponentFixture<OsmMapComponent>;
  let componentRef: ComponentRef<OsmMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeafletModule, OsmMapComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OsmMapComponent);
    component = fixture.componentInstance;
    componentRef = fixture.componentRef;
  });

  // Test if the component creates successfully
  it('should create the OsmMapComponent', () => {
    expect(component).toBeTruthy();
  });

  // Test dynamic width and height signals
  it('should set the correct width and height styles', () => {
    componentRef.setInput('width', '500px');  // assuming input signal uses set() to update
    componentRef.setInput('height', '400px');
    fixture.detectChanges();  // Apply bindings

    const mapDiv = fixture.debugElement.query(By.css('div[style]')).nativeElement;
    expect(mapDiv.style.width).toBe('500px');
    expect(mapDiv.style.height).toBe('400px');
  });

  // Test Leaflet options configuration with signals
  it('should configure Leaflet map options correctly', () => {
    const marker: OsmMapMarker = {
      content: 'Marker Content',
      lat: 10,
      long: 20
    };

    componentRef.setInput('centerLatLong', marker) // updating the signal value
    const options = component.options();
    expect(options.zoom).toBe(16);
    // Check if the center is a LatLng object or LatLngTuple
    if (options.center instanceof Array) {
        // Handle LatLngTuple [lat, lng]
        expect(options.center[0]).toBe(10);   // latitude
        expect(options.center[1]).toBe(20);   // longitude
    } else {
        // Handle LatLng object { lat, lng }
        expect(options.center?.lat).toBe(10);   // latitude
        expect(options.center?.lng).toBe(20);   // longitude
    }
    expect(options.layers?.length).toBeGreaterThan(0);
  });

  // Test marker creation
  it('should create markers based on the input data', () => {
    const marker: OsmMapMarker = {
      content: 'Test Marker',
      lat: 50,
      long: 40
    };
    componentRef.setInput('centerLatLong', marker);  // updating the signal value

    const leafletMarker = component.getMarkers();
    expect(leafletMarker.getLatLng().lat).toBe(50);
    expect(leafletMarker.getLatLng().lng).toBe(40);
    expect(leafletMarker.getPopup()?.getContent()).toBe('Test Marker');
  });

  // Test layers creation
  it('should create map layers with TileLayer and marker', () => {
    const layers = component.getLayers();
    expect(layers.length).toBe(2); // One for tile layer, one for marker
    expect(layers[0]).toBeInstanceOf(TileLayer);
    expect(layers[1]).toBeInstanceOf(Marker);
  });
});
