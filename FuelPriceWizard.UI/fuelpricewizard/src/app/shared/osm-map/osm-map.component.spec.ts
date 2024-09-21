import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmMapComponent } from './osm-map.component';

describe('OsmMapComponent', () => {
  let component: OsmMapComponent;
  let componentRef: ComponentRef<OsmMapComponent>;
  let fixture: ComponentFixture<OsmMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OsmMapComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OsmMapComponent);
    component = fixture.componentInstance;
    componentRef = fixture.componentRef;

    componentRef.setInput("width", "100");
    componentRef.setInput("height", "1ß0");

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
