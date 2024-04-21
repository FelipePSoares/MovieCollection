import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable, filter, map, switchMap } from 'rxjs';
import { MovieCardDto } from 'src/app/core/components/models/movie-card-dto';
import { MovieCardComponent } from 'src/app/core/components/movie-card/movie-card.component';
import { User } from 'src/app/core/models/User';
import { Movie } from 'src/app/core/models/movie';
import { MovieService } from 'src/app/core/services/movie.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-search-movie',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MovieCardComponent
  ],
  templateUrl: './search-movie.component.html',
  styleUrl: './search-movie.component.css'
})
export class SearchMovieComponent implements OnInit {
  public titleControl!: FormControl;
  public searchResults$!: Observable<MovieCardDto[]>;
  user!: User;
  
  constructor(
    private userService: UserService,
    private movieService: MovieService, 
    private formBuilder: FormBuilder,
  ) {}

  ngOnInit() {
    this.titleControl = this.formBuilder.control('');
    this.titleControl.valueChanges
      .pipe(switchMap(searchString => this.searchResults$ = this.searchMovies(searchString)
      )).subscribe();

      this.searchResults$ = this.searchMovies("");

    this.userService.loggedUser$
      .subscribe(res => this.user = res);
  }

  searchMovies(searchString: string): Observable<MovieCardDto[]> {
    return this.movieService
          .search(searchString)
          .pipe(map(movies => movies.map((movie: Movie) => {
            var movieDto = new MovieCardDto();
            movieDto.id = movie.id;
            movieDto.title = movie.title;
            movieDto.description = movie.description;
            movieDto.duration = movie.duration;
            movieDto.releaseYear = movie.releaseYear;
            movieDto.genres = movie.genres;
            movieDto.onUserCollection = this.movieAlreadyAddedToUserCollection(movie.id)
            return movieDto;
          })))
  }

  movieAlreadyAddedToUserCollection(movieId: string) : boolean {
    return this.user.movieCollection.find(m => m.id == movieId) != undefined;
  }

  removeMovieFromCollection(movieId: string) {
    this.userService.removeMovieFromCollection(movieId)
      .subscribe(res => {
        this.user = res;
        this.searchResults$ = this.searchMovies(this.titleControl.value);
      });
  }
  
  addMovieToCollection(movieId: string): void {
    this.userService.addMovieToCollection(movieId)
      .subscribe(res => this.searchResults$ = this.searchMovies(this.titleControl.value));
  }
}
