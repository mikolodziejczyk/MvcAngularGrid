/// <reference path="../../../../../node_modules/popper.js/index.d.ts" />
import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import Popper from 'popper.js/dist/popper';

@Component({
  selector: 'app-column-selector',
  templateUrl: './column-selector.component.html',
  styleUrls: ['./column-selector.component.scss']
})
export class ColumnSelectorComponent implements OnInit {

  constructor () { }

  isVisible: boolean = false;

  // column selector
  @ViewChild('popup', { static: false }) private popup: ElementRef;

  ngOnInit() {
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
}
