import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable, filter, switchMap } from 'rxjs';
import { User } from 'src/app/core/models/User';
import { Movie } from 'src/app/core/models/movie';
import { MovieService } from 'src/app/core/services/movie.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-search-movie',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './search-movie.component.html',
  styleUrl: './search-movie.component.css'
})
export class SearchMovieComponent implements OnInit {
  public titleControl!: FormControl;
  public searchResults$!: Observable<Movie[]>;
  user!: User;
  
  constructor(
    private userService: UserService,
    private movieService: MovieService, 
    private formBuilder: FormBuilder,
  ) {}

  ngOnInit() {
    this.titleControl = this.formBuilder.control('');
    this.titleControl.valueChanges
      .pipe(
        switchMap(searchString => this.searchResults$ = this.movieService.search(searchString))
      ).subscribe();

    this.searchResults$ = this.movieService.search("");

    this.userService.loggedUser$
      .subscribe(res => this.user = res);
  }

  movieAlreadyAddedToUserCollection(movieId: string) : boolean{
    return this.user.movieCollection.find(m => m.id == movieId) != undefined;
  }

  removeMovieFromCollection(movieId: string){
    this.userService.removeMovieFromCollection(movieId).subscribe();
  }
  
  addMovieToCollection(movieId: string): void {
    this.userService.addMovieToCollection(movieId).subscribe();
  }
}
