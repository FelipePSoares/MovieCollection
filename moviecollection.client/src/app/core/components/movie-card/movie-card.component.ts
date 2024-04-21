import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TimeSpan } from '../../models/TimeSpan';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faMinus, faPlus } from '@fortawesome/free-solid-svg-icons';
import { MovieCardDto } from '../models/movie-card-dto';

@Component({
  selector: 'app-movie-card',
  standalone: true,
  imports: [RouterLink, FontAwesomeModule],
  templateUrl: './movie-card.component.html',
  styleUrl: './movie-card.component.css'
})
export class MovieCardComponent {
  faMinus = faMinus;
  faPlus = faPlus

  @Input({ required: true }) movie!: MovieCardDto;
  @Input() isLoggedUser!: boolean;
  @Output() onRemove: EventEmitter<any> = new EventEmitter();
  @Output() onAdd: EventEmitter<any> = new EventEmitter();

  remove(): void {
    this.onRemove.emit();
  }

  add(): void {
    this.onAdd.emit();
  }

  formatDuration(duration: string): TimeSpan{
    var splitStrs = duration.split(':');

   return TimeSpan.fromTime(
     parseInt(splitStrs[0]),
     parseInt(splitStrs[1])
   );
  }
}
