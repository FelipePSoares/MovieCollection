import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, concatMap, map } from 'rxjs';
import { User } from '../models/User';

const USER_DATA = "user_data";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private loggedUser: Subject<User> = new BehaviorSubject<User>(new User());
  loggedUser$: Observable<User> = this.loggedUser.asObservable();

  constructor(private http: HttpClient) {
    const user = localStorage.getItem(USER_DATA);

    if (user) {
      this.loggedUser.next(JSON.parse(user));
    }
  }

  public refreshUserInfo(): Observable<User> {
    return this.http.get<User>('/api/users/profile', {
      observe: 'body',
      responseType: 'json'
    }).pipe(map(user => {
      this.loggedUser.next(user);
      localStorage.setItem(USER_DATA, JSON.stringify(user));
      return user;
    }));
  }

  public removeUserInfo() {
    this.loggedUser.next(new User());
    localStorage.removeItem(USER_DATA);
  }

  public setUserInfo(firstName: string, lastName: string): Observable<User> {
    return this.http.put('/api/users/', {
      firstName: firstName,
      lastName: lastName
    }).pipe(concatMap(res => {
      return this.refreshUserInfo();
    }));
  }
  
  public getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users/', {
      observe: 'body',
      responseType: 'json'
    });
  }
  
  public getUserById(userId: string): Observable<User> {
    return this.http.get<User>('/api/users/' + userId, {
      observe: 'body',
      responseType: 'json'
    });
  }
}
