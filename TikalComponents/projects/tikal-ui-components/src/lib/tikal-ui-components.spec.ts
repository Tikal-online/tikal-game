import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TikalUiComponents } from './tikal-ui-components';

describe('TikalUiComponents', () => {
  let component: TikalUiComponents;
  let fixture: ComponentFixture<TikalUiComponents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TikalUiComponents],
    }).compileComponents();

    fixture = TestBed.createComponent(TikalUiComponents);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
