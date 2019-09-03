import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
// tslint:disable-next-line:max-line-length
import { GridOptions, ValueFormatterParams, ICellRendererParams, RowDoubleClickedEvent, IDatasource, IGetRowsParams } from 'ag-grid-community';
import { formatDate } from '@angular/common';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Debouncer } from './debouncer';
import { IDataResponse } from 'AgGridUtilities/lib/IDataResponse';
import { IAgGridDataRequest, PrepareAgGridDataRequest } from 'AgGridUtilities/lib/IAgGridDataRequest';
import { dateFieldFixer } from './dateFieldFixer';
import { localeText_pl } from 'aggridlocale/lib/pl';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {

  constructor (private http: HttpClient) {
  }

  url: string = '/data/ep/page';
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
      cellRenderer: linkRenderer,
      sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'MeterCode', field: 'meterCode',
      cellRenderer: linkRenderer,
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
    rowModelType: 'infinite',
    cacheBlockSize: 100,
    maxBlocksInCache: 20,
    blockLoadDebounceMillis: 100,
  };

  dataSource: IDatasource = {
    getRows: async (params: IGetRowsParams) => {
      let data: IDataResponse;
      const requestBody: IAgGridDataRequest = PrepareAgGridDataRequest(params, this.globalFilter);

      try {
        data = await this.http.post<IDataResponse>(this.url, requestBody).toPromise();
      } catch (e) {
        params.failCallback();
        return;
      }

      // the part replacing ISO date strings with real dates is acually optional,
      // Angular formatDate handles properly an ISO date string as an input
      dateFieldFixer(data.rows, ['startDate', 'endDate', 'endDateNullable']);

      params.successCallback(data.rows, data.count);
    }
  };

  ngOnInit() {
    this.globalFilterControlSubscription =
      this.globalFilterControl.valueChanges.subscribe(x => this.globalFilterControlDebouncer.onChange());
    this.globalFilterControlDebouncer.callback = () => { this.setGlobalFilter(this.globalFilterControl.value); };
  }

  ngOnDestroy(): void {
    this.globalFilterControlSubscription.unsubscribe();
  }

  onGridReady(params: any) {
    this.gridApi = params.api;
    this.gridApi.setDatasource(this.dataSource);
  }

  onRowDoubleClicked = (event: RowDoubleClickedEvent) => {
    if (event.data) {
      // tslint:disable-next-line:no-string-literal
      alert(`Navigate to: ${event.data['id']}.`);
    }
  }

  setGlobalFilter = (filterText: string) => {
    this.globalFilter = filterText;
    this.gridOptions.api.onFilterChanged();
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

function linkRenderer(params: ICellRendererParams): string {
  const text = params.value;
  let r = null;

  if (params.data) {
    // tslint:disable-next-line:no-string-literal
    const id = params.data['id'];
    const url = `/connection/details/` + id;

    if (text) {
      r = `<a href="${url}">${text}</a>`;
    }
  }

  return r;
}
