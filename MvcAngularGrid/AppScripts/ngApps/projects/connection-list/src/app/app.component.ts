import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
// tslint:disable-next-line:max-line-length
import { GridOptions, ValueFormatterParams, ICellRendererParams, RowDoubleClickedEvent, IDatasource, IGetRowsParams, NumberFilter } from 'ag-grid-community';
import { formatDate } from '@angular/common';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Debouncer } from 'mkoUtils/lib/debouncer';
import { IDataResponse } from 'AgGridUtilities/lib/IDataResponse';
import { localizeNumberFilterDecimalSeparator } from 'AgGridUtilities/lib/localizeNumberFilterDecimalSeparator';
import { IAgGridDataRequest, PrepareAgGridDataRequest } from 'AgGridUtilities/lib/IAgGridDataRequest';
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

  dataUrl: string = '/data/ep/page';
  newUrl: string = '/ep/new';
  displayUrl: string = '/ep/details/';

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
      headerName: 'Kodl licznika', field: 'meterCode',
      cellRenderer: (params: ICellRendererParams) => this.linkRenderer(params),
      sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Nazwa', field: 'name', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Taryfa', field: 'tariff', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Firma', field: 'company', sortable: true,
      filter: true,
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Data od', field: 'startDate',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Data do', field: 'endDate',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Moc zamówiona', field: 'orderedCapacity', sortable: true,
      cellClass: ['text-right'],
      filter: 'agNumberColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Data do (n)', field: 'endDateNullable',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Moc zamówiona (n)', field: 'orderedCapacityNullable', sortable: true,
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
        data = await this.http.post<IDataResponse>(this.dataUrl, requestBody).toPromise();
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
