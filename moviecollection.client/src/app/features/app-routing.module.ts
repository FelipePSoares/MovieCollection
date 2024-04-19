import { NgModule } from '@angular/core';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { FirstSignInGuard } from '../core/guards/first-sign-in-guard';
import { AuthGuard } from '../core/guards/auth-guard';
import { LoginComponent } from './authentication/login/login.component';
import { RegisterComponent } from './authentication/register/register.component';
//import { LogoutComponent } from './authentication/logout/logout.component';
//import { FirstSignInComponent } from './authentication/first-sign-in/first-sign-in.component';
//import { ListProjectsComponent } from './project/list-projects/list-projects.component';
//import { AddProjectComponent } from './project/add-project/add-project.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
//  { path: 'logout', component: LogoutComponent, canActivate: mapToCanActivate([AuthGuard]) },
//  { path: 'first-signin', component: FirstSignInComponent, canActivate: mapToCanActivate([AuthGuard]) },
//  { path: 'projects', component: ListProjectsComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) },
//  { path: 'add-project', component: AddProjectComponent, canActivate: mapToCanActivate([AuthGuard, FirstSignInGuard]) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
