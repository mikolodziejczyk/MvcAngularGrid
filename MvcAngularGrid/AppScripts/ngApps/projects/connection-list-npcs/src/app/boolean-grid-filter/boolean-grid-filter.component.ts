import { Component, OnInit } from '@angular/core';
import { IFilterParams, RowNode, IDoesFilterPassParams } from 'ag-grid-community';

@Component({
  selector: 'app-boolean-grid-filter',
  templateUrl: './boolean-grid-filter.component.html',
  styleUrls: ['./boolean-grid-filter.component.scss']
})
export class BooleanGridFilterComponent implements OnInit {

  constructor() { }


  private params: IFilterParams;
  private valueGetter: (rowNode: RowNode) => any;
  // tslint:disable-next-line:ban-types
  private hidePopup: Function = null;
  // raw value from checkboxes
  public inputValue: string = '';
  // cooked value from checkboxes
  public value: boolean;
  public hasValue: boolean;

  ngOnInit() {
  }


  agInit(params: IFilterParams): void {
      this.params = params;
      this.valueGetter = params.valueGetter;
  }

  isFilterActive(): boolean {
      return this.hasValue;
  }

  doesFilterPass(params: IDoesFilterPassParams): boolean {
      const rowBoolean = this.valueGetter(params.node) as boolean;

      // console.log(
      //     `does pass: ${this.valueGetter(
      //         params.node
      //     )}, rowB ${rowBoolean}, b ${this.value}`
      // );

      return rowBoolean === this.value;

  }

  getModel(): any {
      console.log('getModel() called.');

      let r: any = null;

      if (this.hasValue) {
      r = {
          filterType: 'boolean',
          type: 'equals',
          filter: this.value,
          };
      }

      return r;
  }

  setModel(model: any): void {
      console.log(`setModel() with  ${JSON.stringify(model)} called.`);
      if (model) {
      this.hasValue = true;
      this.value = model.value;
      }
  }

  // noinspection JSMethodCanBeStatic
  componentMethod(message: string): void {
      // alert(`Alert from PartialMatchFilterComponent ${message}`);
      console.log(`The current value is ${this.inputValue}`);
      console.log(`The current cooked value is ${this.value}, hasValue ${this.hasValue}`);
  }

  onChange(newValue): void {
      if (newValue) {
          this.value = (newValue === 'true');
          this.hasValue = true;
      } else {
          this.hasValue = false;
      }

      this.params.filterChangedCallback();
      if (this.hidePopup) {
         this.hidePopup();
      }
  }

  // tslint:disable-next-line:ban-types
  afterGuiAttached(params?: {hidePopup?: Function}): void {
      if (params) {
          this.hidePopup = params.hidePopup;
      }
  }

}
