import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NavBarComponent } from '../core/components/nav-bar/nav-bar.component';
import { SpinnerComponent } from '../core/components/spinner/spinner.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [],
  imports: [
    NavBarComponent,
    SpinnerComponent,
    AppComponent,
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgbModule,
    RouterModule
  ],
  providers: [],
  bootstrap: []
})
export class AppModule { }
