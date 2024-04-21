import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { User } from '../../models/User';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div>
      <a [routerLink]="['/users/',user.id]">
          <svg class="bd-placeholder-img rounded" width="150" height="150" xmlns="http://www.w3.org/2000/svg" role="img" preserveAspectRatio="xMidYMid slice" focusable="false"><title>Placeholder</title><rect width="100%" height="100%" fill="#868e96"></rect></svg>
          <p>{{ user.firstName + " " + user.lastName }}</p>
      </a>
    </div>
  `
})
export class UserCardComponent {
  @Input({ required: true }) user!: User;
}
