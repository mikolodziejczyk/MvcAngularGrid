import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
// tslint:disable-next-line:max-line-length
import { GridOptions, ValueFormatterParams, ICellRendererParams, RowDoubleClickedEvent, IDatasource, IGetRowsParams } from 'ag-grid-community';
import { formatDate } from '@angular/common';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Debouncer } from 'mkoUtils/lib/debouncer';
import { localizeNumberFilterDecimalSeparator } from 'AgGridUtilities/lib/localizeNumberFilterDecimalSeparator';
import { dateFieldFixer } from 'mkoUtils/lib/dateFieldFixer';
import { localeText_pl } from 'aggridlocale/lib/pl';
import { ColumnSelectorComponent } from './column-selector/column-selector.component';
import { ICheckboxEntry } from './column-selector/iCheckBoxEntry';

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
      headerName: 'Data do (N)', field: 'endDateNullable',
      cellClass: ['text-center'],
      valueFormatter: gridDateFormatter,
      sortable: true,
      filter: 'agDateColumnFilter',
      filterParams: { suppressAndOrCondition: true }
    },
    {
      headerName: 'Moc zamówiona (N)', field: 'orderedCapacityNullable', sortable: true,
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

    // the functions reference below don't preserve this so that we add this preservation
    isExternalFilterPresent: () => this.isExternalFilterPresent(),
    doesExternalFilterPass: (node: any) => this.doesExternalFilterPass(node)
  };


  @ViewChild('columnSelectorPopup', { static: false }) private columnSelectorPopup: ColumnSelectorComponent;

  selectColumns = (event: UIEvent) => {
    if (this.columnSelectorPopup.isVisible) {
      this.columnSelectorPopup.close();

      // tslint:disable-next-line:prefer-for-of
      for (const entry of this.columnSelectorPopup.entries) {
        this.gridColumnApi.setColumnVisible(entry.id, entry.value);
      }

    } else {

      const entries: ICheckboxEntry[] = this.gridColumnApi.getAllGridColumns()
        // tslint:disable-next-line:only-arrow-functions
        .map(function(x) { return { id: x.colId, label: x.colDef.headerName, value: x.visible }; });

      this.columnSelectorPopup.entries = entries;

      this.columnSelectorPopup.open(event.target as HTMLElement);
    }
  }

  async getData() {
    let data: any[];

    try {
      data = await this.http.get<[]>(this.dataUrl).toPromise();
    } catch (e) {
      alert('Loading data failed.');
      return;
    }

    // the part replacing ISO date strings with real dates is acually optional,
    // Angular formatDate handles properly an ISO date string as an input
    dateFieldFixer(data as [], ['startDate', 'endDate', 'endDateNullable']);

    // now we want to create ["searchValue"] for every row, that is concatenated lower-case text

    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < data.length; i++) {
      data[i].searchValue = getRowSearchValue(data[i]);
    }

    this.rowData = data;
  }

  ngOnInit() {
    // here the global filter is applied client-side, with few rows we can have shorter debounce time
    this.globalFilterControlDebouncer.timeout = 300;

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
    this.gridColumnApi = params.columnApi;
  }

  onRowDoubleClicked = (event: RowDoubleClickedEvent) => {
    if (event.data) {
      const id = event.data.id;
      const url = this.displayUrl + id.toString();
      window.location.href = url;
    }
  }

  /** Called when the text value in the global fiter input changes, debounced */
  setGlobalFilter = (filterText: string) => {
    if (filterText) {
      filterText = filterText.toLocaleLowerCase();
    }
    this.globalFilter = filterText;
    this.gridApi.onFilterChanged();
  }

  /** Whether the global filter (called by the ag grid "external") is applied */
  isExternalFilterPresent(): boolean {
    return !!this.globalFilter;
  }

  /** Whether a particular node passes the global ("external") filter */
  doesExternalFilterPass(node: any): boolean {
    // the actual row is in node.data
    // we want the searchValue from it (note: searchValue is generated client-side)
    const s = node.data.searchValue as string;
    // the filter passes if the searchValue contains the global filter phrase, this.globalFilter is already lower case
    return s.indexOf(this.globalFilter) !== -1;
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

/** Gets a value to be searched against for a row. This is concatenated text from all text columns, converted to locale lowercase.
 * The value is then stored in the row, like any other column.
 */
function getRowSearchValue(row: any) {
  let searchValue: string = row.ppe || '';
  searchValue += (row.meterCode || '') + ' ';
  searchValue += (row.name || '') + ' ';
  searchValue += (row.tariff || '') + ' ';
  searchValue += (row.company || '') + ' ';
  searchValue = searchValue.toLocaleLowerCase();
  return searchValue;
}


localizeNumberFilterDecimalSeparator();


