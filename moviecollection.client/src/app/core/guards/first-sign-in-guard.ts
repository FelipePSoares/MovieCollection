import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class FirstSignInGuard {
  constructor(
    private userService: UserService,
    private router: Router) { }

  canActivate() {
    return this.isFirstSignIn();
  }

  isFirstSignIn(): Observable<boolean> {
    return this.userService.loggedUser$.pipe(
      map((user) => {
        if (user.hasIncompletedInformation) {
          this.router.navigate(['first-signin']);
        }

        return !user.hasIncompletedInformation;
      }));
  }
}
