import { Injectable } from '@angular/core';
import { IGridState } from 'AgGridUtilities/lib/gridState/iGridState';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GridStateStorageServiceService {

  constructor (private http: HttpClient) { }

  async save(gridId: string, gridState: IGridState) {
    const url: string = '/data/listsettings/savelistsettings';

    const gridStateAsString = JSON.stringify(gridState);

    const dataObject = {
      viewId: gridId,
      settings: gridStateAsString
    };

    return this.http.post(url, dataObject).toPromise();
  }

  async load(gridId: string): Promise<IGridState> {
    const url: string = '/data/listsettings/getlistsettings';

    const dataObject = {
      viewId: gridId
    };

    const valueAsString = await this.http.post<string>(url, dataObject).toPromise();

    let value: IGridState = null;

    if (valueAsString) {
      value = JSON.parse(valueAsString);
    }

    return Promise.resolve(value);
  }
}
