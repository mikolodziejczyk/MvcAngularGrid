import { GridApi, ColumnApi } from 'ag-grid-community';
import { IGridState } from './iGridState';

export class GridStateHelper {
    static getState(gridApi: GridApi, columnApi: ColumnApi): IGridState {
        const r: IGridState = {
            columnState: columnApi.getColumnState(),
            columnGroupState: columnApi.getColumnGroupState(),
            sortModel: gridApi.getSortModel(),
            filterModel: gridApi.getFilterModel()
        };

        return r;
    }

    static setState(gridApi: GridApi, columnApi: ColumnApi, gridState: IGridState) {
        columnApi.setColumnState(gridState.columnState);
        columnApi.setColumnGroupState(gridState.columnGroupState);
        gridApi.setSortModel(gridState.sortModel);
        gridApi.setFilterModel(gridState.filterModel);
    }

    static resetState(gridApi: GridApi, columnApi: ColumnApi) {
        columnApi.resetColumnState();
        columnApi.resetColumnGroupState();
        gridApi.setSortModel(null);
        gridApi.setFilterModel(null);
    }
}

