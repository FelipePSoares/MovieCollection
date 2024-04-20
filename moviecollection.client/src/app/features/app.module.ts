import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NavBarComponent } from '../core/components/nav-bar/nav-bar.component';
import { LoginComponent } from './authentication/login/login.component';
import { RegisterComponent } from './authentication/register/register.component';
import { LogoutComponent } from './authentication/logout/logout.component';
import { FirstSignInComponent } from './authentication/first-sign-in/first-sign-in.component';
import { SpinnerComponent } from '../core/components/spinner/spinner.component';
import { UserComponent } from './user/user/user.component';
import { ListUsersComponent } from './user/list-users/list-users.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    LogoutComponent,
    FirstSignInComponent,
  ],
  imports: [
    NavBarComponent,
    SpinnerComponent,
    AppComponent,
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgbModule,
    RouterModule,
    UserComponent,
    ListUsersComponent
  ],
  providers: [],
  bootstrap: []
})
export class AppModule { }
