import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';

import { AgGridModule } from 'ag-grid-angular';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ColumnSelectorComponent } from './column-selector/column-selector.component';
import { CheckBoxListPopupComponent } from './check-box-list-popup/check-box-list-popup.component';

@NgModule({
  declarations: [
    AppComponent,
    ColumnSelectorComponent,
    CheckBoxListPopupComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridModule.withComponents([])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
