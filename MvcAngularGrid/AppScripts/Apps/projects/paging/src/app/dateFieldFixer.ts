import { parseNullableDate } from './parseNullableDate';

export function dateFieldFixer(rows: [], dateColumns: string[]): void {
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < rows.length; i++) {
        const row: any = rows[i];
        for (const field of dateColumns) {
            row[field] = parseNullableDate(row[field]);
        }
    }
}
