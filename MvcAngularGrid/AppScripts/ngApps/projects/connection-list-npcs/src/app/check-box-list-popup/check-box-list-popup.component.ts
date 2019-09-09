/// <reference path="../../../../../node_modules/popper.js/index.d.ts" />
import { Component, OnInit, ElementRef, ViewChild, Input, OnDestroy, AfterViewInit, forwardRef } from '@angular/core';
import Popper from 'popper.js/dist/popper';
import { ICheckboxEntry } from '../iCheckBoxEntry';
import { NgForm, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-check-box-list-popup',
  templateUrl: './check-box-list-popup.component.html',
  styleUrls: ['./check-box-list-popup.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckBoxListPopupComponent),
      multi: true
    }
  ]
})
export class CheckBoxListPopupComponent implements OnInit, OnDestroy, AfterViewInit, ControlValueAccessor {


  constructor () { }

  isVisible: boolean = false;

  @Input() entries: ICheckboxEntry[] = [
    { id: 'ppe', label: 'Ppe', value: true },
    { id: 'meterCode', label: 'Kod licznika', value: true },
    { id: 'company', label: 'Firma', value: true },
  ];

  // column selector
  @ViewChild('popup', { static: false }) private popup: ElementRef;
  @ViewChild('myControl', { static: false }) myControl: NgForm;

  myControlChangeSubcription: Subscription;

  ngOnInit() {

  }

  ngAfterViewInit(): void {
    this.myControlChangeSubcription = this.myControl.valueChanges.subscribe(x => {
      window.setTimeout( () => { this.propagateChange(this.entries); this.onTouched(); }, 1);
    });
  }

  ngOnDestroy(): void {
    this.myControlChangeSubcription.unsubscribe();
  }


  open = (referenceElement: HTMLElement) => {
    this.isVisible = true;

    const popperInstance = new Popper(referenceElement, this.popup.nativeElement, {
      placement: 'auto'
    });
  }


  close = () => {
    this.isVisible = false;
  }

  writeValue(value: any) {
    if (value === undefined) {
      value = []; // normalize undefined to null
    }
    if (this.entries !== value) {
      this.entries = value;
    }
  }

  propagateChange = (_: any) => { };

  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  onTouched = () => { };

  registerOnTouched(fn) {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
  }

}
