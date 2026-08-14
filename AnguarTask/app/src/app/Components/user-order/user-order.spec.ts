import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserOrder } from './user-order';

describe('UserOrder', () => {
  let component: UserOrder;
  let fixture: ComponentFixture<UserOrder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserOrder]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserOrder);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
