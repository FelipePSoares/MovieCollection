import { HttpErrorResponse } from '@angular/common/http';
import { HttpInterceptorFn } from "@angular/common/http";

import { Observable, catchError, map, of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';

const TOKEN_DATA = "token_data";

export const HttpRequestInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem(TOKEN_DATA);

  if (!token) {
    return next(req).pipe(catchError(x => handleAuthError(x)));
  }

  req = req.clone({
    headers: req.headers.set('Authorization', `Bearer ${JSON.parse(token).accessToken}`),
  });

  return next(req).pipe(catchError(x => handleAuthError(x)));
}

function handleAuthError(err: HttpErrorResponse): Observable <any> {
  var router = inject(Router);
  var authService = inject(AuthService);

  if((err.status === 401 || err.status === 403) && !err.url?.includes('logout')) {
    authService.signOut();
    router.navigate(['login']);

    return of(err.message);
  }

  return throwError(() => err);
}
