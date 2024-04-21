/// <reference types="@angular/localize" />

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppComponent } from './app/features/app.component';


platformBrowserDynamic().bootstrapModule(AppComponent)
  .catch(err => console.error(err));
