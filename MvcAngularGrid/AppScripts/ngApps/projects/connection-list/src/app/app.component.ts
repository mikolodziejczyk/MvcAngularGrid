import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
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
import { BooleanGridFilterComponent, GridStateStorageService } from 'ag-grid-support-lib';
import { CheckBoxListPopupComponent } from 'mko-ng-components';
import { ToastrService } from 'ngx-toastr';
import { GridStateHelper } from 'AgGridUtilities/lib/gridState/gridStateHelper';
import { IGridState } from 'AgGridUtilities/lib/gridState/iGridState';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {

  constructor (private http: HttpClient, private statestorage: GridStateStorageService, private toastr: ToastrService) {
    this.frameworkComponents = { booleanGridFilter: BooleanGridFilterComponent };
  }

  dataUrl: string = '/data/ep/page';
  newUrl: string = '/ep/new';
  displayUrl: string = '/ep/details/';
  /** The unique identifier of this grid, for loading and saving state. */
  gridId: string = 'list/connections/index?server-side';

  globalFilterControl: FormControl = new FormControl();
  globalFilterControlSubscription: Subscription;
  globalFilterControlDebouncer: Debouncer = new Debouncer();

  localeText = localeText_pl;
  globalFilter = '';

  frameworkComponents: any;

  private gridApi;
  private gridColumnApi;

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
      headerName: 'Kod licznika', field: 'meterCode',
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
    },
    {
      headerName: 'Aktywne', field: 'isActive',
      sortable: true,
      cellClass: ['text-center'],
      valueFormatter: gridBooleanFormatter,
      filter: 'booleanGridFilter'
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

  columnVisibilityControl: FormControl = new FormControl();
  columnVisibilityControlSubscription: Subscription;
  @ViewChild('columnSelectorPopup', { static: false }) private columnSelectorPopup: CheckBoxListPopupComponent;

  get columnLabels(): string[] {
    const labels: string[] = this.gridColumnApi.getAllGridColumns().map(x => x.colDef.headerName);
    return labels;
  }

  /** This is a button hander. We need event.target for the popup placement */
  selectColumns = (event: UIEvent) => {
    if (this.columnSelectorPopup.isVisible) {
      this.columnSelectorPopup.close();
    } else {
      const values: boolean[] = this.gridColumnApi.getAllGridColumns().map(x => x.visible);

      this.columnSelectorPopup.labels = this.columnLabels;
      this.columnVisibilityControl.setValue(values);

      this.columnSelectorPopup.open(event.target as HTMLElement);
    }
  }

  updateColumnsVisibility = (values: boolean[]) => {
    const columnIds: string[] = this.gridColumnApi.getAllGridColumns().map(x => x.colId);
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < values.length; i++) {
      this.gridColumnApi.setColumnVisible(columnIds[i], values[i]);
    }
  }


  ngOnInit() {
    this.globalFilterControlSubscription =
      this.globalFilterControl.valueChanges.subscribe(x => this.globalFilterControlDebouncer.onChange());
    this.globalFilterControlDebouncer.callback = () => { this.setGlobalFilter(this.globalFilterControl.value); };

    this.columnVisibilityControlSubscription = this.columnVisibilityControl.valueChanges.subscribe(x => this.updateColumnsVisibility(x));
  }

  ngOnDestroy(): void {
    this.globalFilterControlSubscription.unsubscribe();
    this.columnVisibilityControlSubscription.unsubscribe();
  }

  onGridReady(params: any) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    // this.gridApi.setDatasource(this.dataSource);
    this.getGridStateAndData();
  }

  async getGridStateAndData() {
    const gridState: IGridState = await this.statestorage.load(this.gridId);
    if (gridState) {
      GridStateHelper.setState(this.gridApi, this.gridColumnApi, gridState);
    }

    this.gridApi.setDatasource(this.dataSource);
  }

  saveGridState = async () => {
    const gridState = GridStateHelper.getState(this.gridApi, this.gridColumnApi);
    await this.statestorage.save(this.gridId, gridState);
    this.toastr.success('Bieżące ustawienia listy zostały zapisane i będą wczytywane automatycznie przy kolejnym uruchomieniu.',
    'Zapisano');
  }

  restoreGridState = async () => {
    const gridState: IGridState = await this.statestorage.load(this.gridId);
    if (gridState) {
      GridStateHelper.setState(this.gridApi, this.gridColumnApi, gridState);
    } else {
      GridStateHelper.resetState(this.gridApi, this.gridColumnApi);
    }
  }

  resetGridState = () => {
    GridStateHelper.resetState(this.gridApi, this.gridColumnApi);
    // tslint:disable-next-line:max-line-length
    this.toastr.info('Przywrócono ustawienia fabryczne listy. Aby przy kolejnym uruchomieniu pojawiły sie one automatycznie, zapisz bieżący widok.',
    'Przywrócono');
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

function gridBooleanFormatter(params: ValueFormatterParams): any {
  const value: boolean = params.value;
  let r: string = null; // note that the value can be null

  if (value === true) {
    r = 'tak';
  }
  if (value === false) {
    r = 'nie';
  }

  return r;
}


localizeNumberFilterDecimalSeparator();
