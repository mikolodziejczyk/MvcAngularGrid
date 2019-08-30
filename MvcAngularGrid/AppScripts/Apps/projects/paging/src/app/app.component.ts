import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions, IDatasource, IGetRowsParams, ValueFormatterParams } from 'ag-grid-community';
import { formatDate } from '@angular/common';

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
    {
      headerName: 'Ppe', field: 'ppe', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true, filterOptions: ['contains', 'notContains', 'startsWith', 'equals', 'notEqual'] }
    },
    {
      headerName: 'CeterCode', field: 'meterCode', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true, filterOptions: ['contains', 'notContains', 'startsWith', 'equals', 'notEqual'] }
    },
    {
      headerName: 'Name', field: 'name', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true, filterOptions: ['contains', 'notContains', 'startsWith', 'equals', 'notEqual'] }
    },
    {
      headerName: 'Tariff', field: 'tariff', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true, filterOptions: ['contains', 'notContains', 'startsWith', 'equals', 'notEqual'] }
    },
    {
      headerName: 'Company', field: 'company', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true, filterOptions: ['contains', 'notContains', 'startsWith', 'equals', 'notEqual'] }
    },
    {
      headerName: 'Start date', field: 'startDate',
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'End date', field: 'endDate',
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Ordered capacity', field: 'orderedCapacity', sortable: true,
      filter: 'agNumberColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'End date nullable', field: 'endDateNullable',
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Ordered capacity nullable', field: 'orderedCapacityNullable', sortable: true,
      filter: 'agNumberColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    }
  ];

  rowData: any;

  gridOptions: GridOptions = {
    // pagination: true,
    rowModelType: 'infinite',
    cacheBlockSize: 100, // you can have your custom page size
    maxBlocksInCache: 10,
    blockLoadDebounceMillis: 100,
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

      let data: IDataResponse;

      try {
        data = await this.loadData(params);
      } catch (e) {
        alert('Loading error');
      }

      // the part replacing ISO date strings with real dates is acually optional,
      // Angular formatDate handles properly a ISO date string as an input

      // tslint:disable-next-line:prefer-for-of
      for (let i = 0; i < data.rows.length; i++) {
        const row: any = data.rows[i];
        row.startDate = parseNullableDate(row.startDate);
        row.endDate = parseNullableDate(row.endDate);
        row.endDateNullable = parseNullableDate(row.endDateNullable);
      }

      const pageSize = 20;

      const pages = Math.floor(data.count / pageSize);

      console.log(`calling success with ${JSON.stringify(data.rows)} and page count ${pages}.`);

      params.successCallback(data.rows, data.count);



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

  async loadData(params: IGetRowsParams): Promise<IDataResponse> {
    let r: any;
    try {
      const url = `/data/ep/page`;

      console.log(`Fetching from ${url} with ${JSON.stringify(params)}`);
      r = await this.http.post(url, params).toPromise();
    } catch (e) {
      alert('Unable to load data.');
    }

    return r as IDataResponse;
  }

}

interface IDataResponse {
  rows: [];
  count: number;
}

function parseNullableDate(input: string): Date | null {
  let r: Date | null = null;
  if (input) {
    r = new Date(input);
  }

  return r;
}

function gridDateFormatter(params: ValueFormatterParams): any {
  const date: Date = params.value;
  let r: string = null;
  if (date) {
    r = formatDate(date, 'yyyy-MM-dd', 'en-US');
  }
  return r;
}
