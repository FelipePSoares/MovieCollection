import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'src/app/core/models/User';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-user',
  standalone: true,
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})

export class UserComponent {
  isLoggedUser: boolean = false;
  user!: User;
  
  constructor(private userService: UserService, private router: Router) { }

  @Input() 
  set id(id: string) {
    if (id != undefined){
      this.userService.getUserById(id)
        .subscribe(res => this.user = res);
      this.isLoggedUser = false;
    } else {
      this.userService.loggedUser$
        .subscribe(res => this.user = res);
      this.isLoggedUser = true;
    }
  }

  removeMovieFromCollection(index: number){
    this.userService.removeMovieFromCollection(index).subscribe();
  }
  
  addMovieToCollection(): void {
    this.router.navigate(['search-movie']);
  }
}
