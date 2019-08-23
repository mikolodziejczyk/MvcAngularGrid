import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit  {

  constructor(private http: HttpClient) {

  }

  title = 'Apps';

  columnDefs = [
    {headerName: 'Ppe', field: 'ppe', sortable: true, filter: true },
    {headerName: 'CeterCode', field: 'meterCode', sortable: true, filter: true },
    {headerName: 'Name', field: 'name', sortable: true, filter: true},
    {headerName: 'Tariff', field: 'tariff', sortable: true, filter: true},
    {headerName: 'Company', field: 'company', sortable: true, filter: true}
  ];

  rowData: any;


  ngOnInit() {
      this.loadData();
  }

  async loadData() {
    try {
    this.rowData = await this.http.get('/data/ep/index').toPromise();
    } catch (e) {
      alert('Unable to load data.');
    }
  }
}
