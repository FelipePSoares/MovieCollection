import { Routes, mapToCanActivate } from '@angular/router';
import { FirstSignInGuard } from '../core/guards/first-sign-in-guard';
import { AuthGuard } from '../core/guards/auth-guard';
import { LoginComponent } from './authentication/login/login.component';
import { RegisterComponent } from './authentication/register/register.component';
import { LogoutComponent } from './authentication/logout/logout.component';
import { FirstSignInComponent } from './authentication/first-sign-in/first-sign-in.component';
import { UserComponent } from './user/user/user.component';
import { ListUsersComponent } from './user/list-users/list-users.component';
import { SearchMovieComponent } from './movie/search-movie/search-movie.component';
import { AddMovieComponent } from './movie/add-movie/add-movie.component';

export const routes: Routes = [
  { path: '', redirectTo: 'profile', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'logout', component: LogoutComponent, canActivate: mapToCanActivate([AuthGuard]) },
  { path: 'first-signin', component: FirstSignInComponent, canActivate: mapToCanActivate([AuthGuard]) },
  { path: 'profile', component: UserComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) },
  { path: 'users', component: ListUsersComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) },
  { path: 'users/:id', component: UserComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) },
  { path: 'search-movie', component: SearchMovieComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) },
  { path: 'add-movie', component: AddMovieComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) }
];
