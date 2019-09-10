import { Injectable } from '@angular/core';
import { IGridState } from 'AgGridUtilities/lib/gridState/iGridState';

@Injectable({
  providedIn: 'root'
})
export class GridStateStorageServiceService {

  constructor() { }

  async save(gridId: string, gridState: IGridState) {
    const gridStateAsString = JSON.stringify(gridState);
    window.localStorage.setItem(gridId, gridStateAsString);
    return Promise.resolve();
  }

  async load(gridId: string): Promise<IGridState> {
    const valueAsString = window.localStorage.getItem(gridId);
    let value: IGridState = null;
    if (valueAsString) {
      value = JSON.parse(valueAsString);
    }

    return Promise.resolve(value);
  }
}
