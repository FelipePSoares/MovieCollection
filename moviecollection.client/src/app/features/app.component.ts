import { Component, OnInit } from '@angular/core';
import { AuthService } from '../core/services/auth.service';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterOutlet, provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app-routing.module';
import { NavBarComponent } from '../core/components/nav-bar/nav-bar.component';
import { SpinnerComponent } from '../core/components/spinner/spinner.component';
import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpRequestInterceptor } from '../core/interceptor/http-request-interceptor';
import { LoadingInterceptor } from '../core/interceptor/loading.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    NavBarComponent,
    SpinnerComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(public authService: AuthService) { }

  ngOnInit() {
  }
}

bootstrapApplication(AppComponent, {
  providers: [
    provideAnimations(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(
      withInterceptors([HttpRequestInterceptor, LoadingInterceptor]))],
});
