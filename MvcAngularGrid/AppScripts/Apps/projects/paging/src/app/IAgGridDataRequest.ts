import { IGetRowsParams } from 'ag-grid-community';

/**
 * Represents an object sent to the server when requesting row data.
 * This is similar to IGetRowsParams but doesn't include functions and include globalFilter.
 */
export interface IAgGridDataRequest {
    startRow: number;
    endRow: number;
    sortModel: any;
    filterModel: any;
    globalFilter: string;
}

export function PrepareAgGridDataRequest(params: IGetRowsParams, globalFilter: string) {
    const r: IAgGridDataRequest = {
        startRow: params.startRow,
        endRow: params.endRow,
        sortModel: params.sortModel,
        filterModel: params.filterModel,
        // tslint:disable-next-line:object-literal-shorthand
        globalFilter: globalFilter
    };

    return r;
}
