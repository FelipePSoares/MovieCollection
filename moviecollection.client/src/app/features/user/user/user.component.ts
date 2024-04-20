import { Component, Input, OnInit } from '@angular/core';
import { User } from 'src/app/core/models/User';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-user',
  standalone: true,
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})

export class UserComponent {
  user: User = new User();
  
  constructor(private userService: UserService) { }

  @Input() 
  set id(id: string) {
    this.userService.getUserById(id)
      .subscribe(res => this.user = res);
  }
}
