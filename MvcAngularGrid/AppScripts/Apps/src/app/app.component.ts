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
    {headerName: 'Ppe', field: 'ppe' },
    {headerName: 'CeterCode', field: 'meterCode' },
    {headerName: 'Name', field: 'name'},
    {headerName: 'Tariff', field: 'tariff'},
    {headerName: 'Company', field: 'company'}
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
