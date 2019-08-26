import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions, IDatasource, IGetRowsParams } from 'ag-grid-community';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  constructor (private http: HttpClient) {

  }

  title = 'Apps';

  private gridApi;

  columnDefs = [
    { headerName: 'Ppe', field: 'ppe', sortable: true, filter: true },
    { headerName: 'CeterCode', field: 'meterCode', sortable: true, filter: true },
    { headerName: 'Name', field: 'name', sortable: true, filter: true },
    { headerName: 'Tariff', field: 'tariff', sortable: true, filter: true },
    { headerName: 'Company', field: 'company', sortable: true, filter: true }
  ];

  rowData: any;

  gridOptions: GridOptions = {
    // pagination: true,
    rowModelType: 'infinite',
    cacheBlockSize: 20, // you can have your custom page size
    // paginationPageSize: 20 // pagesize
  };


  dataSource: IDatasource = {
    getRows: async (params: IGetRowsParams) => {

      // Use startRow and endRow for sending pagination to Backend
      // params.startRow : Start Page
      // params.endRow : End Page

      // replace this.apiService with your Backend Call that returns an Observable
      // this.apiService().subscribe(response => {

      //   params.successCallback(
      //     response.data, response.totalRecords
      //   );

      // })

      console.log(`getRows called with ${params.startRow} to ${params.endRow}.`);

      let data: [];

      try {
        data = await this.loadData();
      } catch (e) {
        alert('Loading error');
      }

      const pageSize = 20;

      const r = data.slice(pageSize * params.startRow, pageSize * params.endRow);
      const pages = Math.floor(data.length / pageSize);

      console.log(`calling success with ${JSON.stringify(r)} and page count ${pages}.`);

      params.successCallback(r, pages);



    }
  };

  ngOnInit() {
    // this.loadData();
  }

  onGridReady(params: any) {
    console.log(`onGridReady called.`);
    this.gridApi = params.api;
    this.gridApi.setDatasource(this.dataSource); // replace dataSource with your datasource
  }

  async loadData(): Promise<[]> {
    let r: any;
    try {
       r = await this.http.get('/data/ep/index').toPromise();
    } catch (e) {
      alert('Unable to load data.');
    }

    return r as [];
  }

}
