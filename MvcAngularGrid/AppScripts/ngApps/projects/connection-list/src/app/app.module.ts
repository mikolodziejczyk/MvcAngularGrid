import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';

import { AgGridModule } from 'ag-grid-angular';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BooleanGridFilterComponent, BooleanGridFilterModule } from 'ag-grid-support-lib/esm2015/public-api';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    BrowserAnimationsModule,
    BooleanGridFilterModule,
    AgGridModule.withComponents([BooleanGridFilterComponent])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
