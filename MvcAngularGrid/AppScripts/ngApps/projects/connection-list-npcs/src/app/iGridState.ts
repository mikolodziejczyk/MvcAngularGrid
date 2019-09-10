import { ColumnState } from 'ag-grid-community/dist/lib/columnController/columnController';

export interface IGridState {
    columnState: ColumnState[];
    columnGroupState: any[];
    sortModel: any[];
    filterModel: any;
}
