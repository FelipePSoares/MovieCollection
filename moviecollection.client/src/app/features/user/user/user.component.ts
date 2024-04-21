import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { Observable, map } from 'rxjs';
import { MovieCardDto } from 'src/app/core/components/models/movie-card-dto';
import { MovieCardComponent } from 'src/app/core/components/movie-card/movie-card.component';
import { User } from 'src/app/core/models/User';
import { Movie } from 'src/app/core/models/movie';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    MovieCardComponent,
    FontAwesomeModule
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})

export class UserComponent {
  faPlus = faPlus;
  isLoggedUser: boolean = false;
  user!: User;
  public movies!: MovieCardDto[];
  
  constructor(private userService: UserService, private router: Router) { }

  @Input() 
  set id(id: string) {
    if (id != undefined){
      this.userService.getUserById(id)
        .subscribe(res => {
          this.user = res;
          this.movies = this.mapMovieToDto(res.movieCollection)
        });
      this.isLoggedUser = false;
    } else {
      this.userService.loggedUser$
        .subscribe(res => {
          this.user = res;
          this.movies = this.mapMovieToDto(res.movieCollection)
        });
      this.isLoggedUser = true;
    }
  }

  mapMovieToDto(movies: Movie[]): MovieCardDto[]{
    if (movies == undefined)
      return <MovieCardDto[]>[];

    return movies.map((movie: Movie) => {
      var movieDto = new MovieCardDto();
      movieDto.id = movie.id;
      movieDto.title = movie.title;
      movieDto.description = movie.description;
      movieDto.duration = movie.duration;
      movieDto.releaseYear = movie.releaseYear;
      movieDto.genres = movie.genres;
      movieDto.onUserCollection = true
      return movieDto;
    });
  }

  removeMovieFromCollection(movieId: string){
    this.userService.removeMovieFromCollection(movieId).subscribe();
  }
  
  addMovieToCollection(): void {
    this.router.navigate(['search-movie']);
  }
}