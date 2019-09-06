import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
// tslint:disable-next-line:max-line-length
import { GridOptions, ValueFormatterParams, ICellRendererParams, RowDoubleClickedEvent, IDatasource, IGetRowsParams } from 'ag-grid-community';
import { formatDate } from '@angular/common';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Debouncer } from 'mkoUtils/lib/debouncer';
import { IDataResponse } from 'AgGridUtilities/lib/IDataResponse';
import { localizeNumberFilterDecimalSeparator } from 'AgGridUtilities/lib/localizeNumberFilterDecimalSeparator';
import { dateFieldFixer } from 'mkoUtils/lib/dateFieldFixer';
import { localeText_pl } from 'aggridlocale/lib/pl';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {

  constructor (private http: HttpClient) {
  }

  dataUrl: string = '/data/epnp/connections';
  newUrl: string = '/epnp/new';
  displayUrl: string = '/epnp/details/';

  rowData: any;

  globalFilterControl: FormControl = new FormControl();
  globalFilterControlSubscription: Subscription;
  globalFilterControlDebouncer: Debouncer = new Debouncer();

  localeText = localeText_pl;
  globalFilter = '';

  private gridApi;

  defaultColDef = {
    resizable: true
  };

  columnDefs = [
    {
      headerName: 'Ppe', field: 'ppe',
      cellRenderer: (params: ICellRendererParams) => this.linkRenderer(params),
      sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'MeterCode', field: 'meterCode',
      cellRenderer: (params: ICellRendererParams) => this.linkRenderer(params),
      sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Name', field: 'name', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Tariff', field: 'tariff', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Company', field: 'company', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Start date', field: 'startDate',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'End date', field: 'endDate',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Ordered capacity', field: 'orderedCapacity', sortable: true,
      cellClass: ['text-right'],
      filter: 'agNumberColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'End date nullable', field: 'endDateNullable',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Ordered capacity nullable', field: 'orderedCapacityNullable', sortable: true,
      cellClass: ['text-right'],
      filter: 'agNumberColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    }
  ];

  gridOptions: GridOptions = {
    // rowModelType: 'infinite',
    // cacheBlockSize: 100,
    // maxBlocksInCache: 20,
    // blockLoadDebounceMillis: 100,
  };

  async getData() {
    let data: [];

    try {
      data = await this.http.get<[]>(this.dataUrl).toPromise();
    } catch (e) {
      alert('Loading data failed.');
      return;
    }

    // the part replacing ISO date strings with real dates is acually optional,
    // Angular formatDate handles properly an ISO date string as an input
    dateFieldFixer(data, ['startDate', 'endDate', 'endDateNullable']);

    this.rowData = data;
  }

  ngOnInit() {
    this.globalFilterControlSubscription =
      this.globalFilterControl.valueChanges.subscribe(x => this.globalFilterControlDebouncer.onChange());
    this.globalFilterControlDebouncer.callback = () => { this.setGlobalFilter(this.globalFilterControl.value); };

    this.getData();
  }

  ngOnDestroy(): void {
    this.globalFilterControlSubscription.unsubscribe();
  }

  onGridReady(params: any) {
    this.gridApi = params.api;
  }

  onRowDoubleClicked = (event: RowDoubleClickedEvent) => {
    if (event.data) {
      const id = event.data.id;
      const url = this.displayUrl + id.toString();
      window.location.href = url;
    }
  }

  setGlobalFilter = (filterText: string) => {
    this.globalFilter = filterText;
    this.gridApi.onFilterChanged();
  }

  linkRenderer(params: ICellRendererParams): string {
    const text = params.value;
    let r = null;

    if (params.data) {
      const id = params.data.id;
      const url = this.displayUrl + id.toString();

      if (text) {
        r = `<a href="${url}">${text}</a>`;
      }
    }

    return r;
  }


}

function gridDateFormatter(params: ValueFormatterParams): any {
  const date: Date = params.value;
  let r: string = null;
  if (date) {
    r = formatDate(date, 'yyyy-MM-dd', 'en-US');
  }
  return r;
}


localizeNumberFilterDecimalSeparator();
