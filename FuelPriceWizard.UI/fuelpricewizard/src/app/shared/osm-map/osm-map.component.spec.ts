import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OsmMapComponent } from './osm-map.component';

describe('OsmMapComponent', () => {
  let component: OsmMapComponent;
  let fixture: ComponentFixture<OsmMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OsmMapComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OsmMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
