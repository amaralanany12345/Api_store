import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllOrders } from './all-orders';

describe('AllOrders', () => {
  let component: AllOrders;
  let fixture: ComponentFixture<AllOrders>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllOrders]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllOrders);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
