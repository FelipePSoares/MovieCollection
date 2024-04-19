import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { NavBarComponent } from '../core/components/nav-bar/nav-bar.component';
import { HttpRequestInterceptor } from '../core/interceptor/http-request-interceptor';
import { LoginComponent } from './authentication/login/login.component';
import { RegisterComponent } from './authentication/register/register.component';
import { LogoutComponent } from './authentication/logout/logout.component';
import { FirstSignInComponent } from './authentication/first-sign-in/first-sign-in.component';
import { SpinnerComponent } from '../core/components/spinner/spinner.component';
import { LoadingInterceptor } from '../core/interceptor/loading.interceptor';
import { ProfileComponent } from './user/profile/profile.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavBarComponent,
    RegisterComponent,
    LogoutComponent,
    FirstSignInComponent,
    SpinnerComponent,
    ProfileComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [
    [
      { provide: HTTP_INTERCEPTORS, useClass: HttpRequestInterceptor, multi: true },
      { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }
    ]
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
