import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';

import { AppComponent } from './app.component';

import { AgGridModule } from 'ag-grid-angular';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CheckBoxListPopupComponent } from './check-box-list-popup/check-box-list-popup.component';
import { BooleanGridFilterComponent, BooleanGridFilterModule } from 'ag-grid-support-lib/esm2015/public-api';

@NgModule({
  declarations: [
    AppComponent,
    CheckBoxListPopupComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    BrowserAnimationsModule,
    BooleanGridFilterModule,
    AgGridModule.withComponents([BooleanGridFilterComponent]),
    ToastrModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
