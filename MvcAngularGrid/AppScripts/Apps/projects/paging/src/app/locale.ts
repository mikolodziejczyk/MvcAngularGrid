const localeText = {
    // for filter panel
    page: 'strona',
    more: 'więcej',
    to: 'do',
    of: 'z',
    next: 'następna',
    last: 'ostatnia',
    first: 'pierwsza',
    previous: 'poprzedni',
    loadingOoo: 'wczytywanie...',

    // for set filter
    selectAll: 'Wybierz wszystko',
    searchOoo: 'znajdź...',
    blanks: 'puste',

    // for number filter and text filter
    filterOoo: 'filtr...',
    applyFilter: 'Zastosuj filtr...',
    equals: 'równe',
    notEqual: 'różne od',

    // for number filter
    lessThan: 'mniejsze od',
    greaterThan: 'większe od',
    lessThanOrEqual: 'mniejsze lub równe od',
    greaterThanOrEqual: 'większe lub równe',
    inRange: 'w przedziale',

    // for text filter
    contains: 'zawiera',
    notContains: 'nie zawiera',
    startsWith: 'zaczyna się od',
    endsWith: 'kończy się na',

    // filter conditions
    andCondition: 'i',
    orCondition: 'lub',

    // the header of the default group column
    group: 'grupa',

    // tool panel
    columns: 'kolumny',
    filters: 'filtry',
    rowGroupColumns: 'Pivot Cols',
    rowGroupColumnsEmptyMessage: 'drag cols to group',
    valueColumns: 'Value Cols',
    pivotMode: 'Pivot-Mode',
    groups: 'Groups',
    values: 'Values',
    pivots: 'Pivots',
    valueColumnsEmptyMessage: 'drag cols to aggregate',
    pivotColumnsEmptyMessage: 'drag here to pivot',
    toolPanelButton: 'tool panel',

    // other
    noRowsToShow: 'brak wierszy',

    // standard menu
    copy: 'Kopiuj',
    copyWithHeaders: 'Kopiuj z nagówkami',
    ctrlC: 'ctrl + C',
    paste: 'Wklej',
    ctrlV: 'ctrl + V'
};

export function getLocalizedText() {
    return localeText;
}
