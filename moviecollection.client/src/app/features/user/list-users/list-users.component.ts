import { AsyncPipe, CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { UserCardComponent } from 'src/app/core/components/user-card/user-card.component';
import { User } from 'src/app/core/models/User';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-list-users',
  standalone: true,
  imports: [
    CommonModule, 
    RouterLink, 
    AsyncPipe,
    UserCardComponent
  ],
  templateUrl: './list-users.component.html',
  styleUrl: './list-users.component.css'
})
export class ListUsersComponent implements OnInit {
  private users: BehaviorSubject<User[]> = new BehaviorSubject<User[]>([new User()]);
  users$: Observable<User[]> = this.users.asObservable();

  constructor (private userService: UserService) { }
  
  ngOnInit(): void {
    this.userService.getUsers()
      .subscribe(
        {
          next: res => { this.users.next(res); }
        });
  }
}
