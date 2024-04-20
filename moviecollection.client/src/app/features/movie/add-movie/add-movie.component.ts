import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Genre } from 'src/app/core/models/genre';
import { Movie } from 'src/app/core/models/movie';
import { GenreService } from 'src/app/core/services/genre.service';
import { MovieService } from 'src/app/core/services/movie.service';

@Component({
  selector: 'app-add-movie',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-movie.component.html',
  styleUrl: './add-movie.component.css'
})
export class AddMovieComponent implements OnInit{
  genres!: Genre[];
  movieForm!: FormGroup;
  httpErrors = false;
  errors: any;

  constructor(private movieService: MovieService, private genreService: GenreService, private router: Router) { }

  ngOnInit(): void {
    this.movieForm = new FormGroup({
      title: new FormControl('', [Validators.required]),
      description: new FormControl(''),
      ReleaseYear: new FormControl(new Date().getFullYear()),
      Duration: new FormControl(''),
      Genres: new FormControl('')
    });
    
    this.genreService.getAll()
      .subscribe(res => this.genres = res);
  }
  
  saveMovie(){
    if (this.movieForm.valid) {
      const title = this.title?.value;
      const description = this.description?.value;
      const ReleaseYear = this.ReleaseYear?.value;
      const Duration = this.Duration?.value;
      //const Genres = this.Genres?.value;

      var newMovie = <Movie>({
        title: title,
        description: description,
        releaseYear: ReleaseYear,
        duration: Duration,
        //genres: Genres
      });

      this.movieService.addMovie(newMovie).subscribe({
        next: response => {
          this.router.navigate(['/profile']);
        },
        error: error => {
          this.httpErrors = true;
          this.errors = error;
        }
      });
    }
  }
  
  get title() {
    return this.movieForm.get('title');
  }

  get description() {
    return this.movieForm.get('description');
  }

  get ReleaseYear() {
    return this.movieForm.get('ReleaseYear');
  }

  get Duration() {
    return this.movieForm.get('Duration');
  }

  get Genres() {
    return this.movieForm.get('Genres');
  }
}
