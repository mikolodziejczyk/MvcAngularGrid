import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckBoxListPopupComponent } from './check-box-list-popup.component';

describe('CheckBoxListPopupComponent', () => {
  let component: CheckBoxListPopupComponent;
  let fixture: ComponentFixture<CheckBoxListPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckBoxListPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckBoxListPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
