import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Genre } from 'src/app/core/models/genre';
import { Movie } from 'src/app/core/models/movie';
import { GenreService } from 'src/app/core/services/genre.service';
import { MovieService } from 'src/app/core/services/movie.service';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Observable, map, startWith } from 'rxjs';

@Component({
  selector: 'app-add-movie',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    MatFormFieldModule,
    MatChipsModule,
    MatIconModule,
    MatAutocompleteModule
  ],
  templateUrl: './add-movie.component.html',
  styleUrl: './add-movie.component.css'
})
export class AddMovieComponent implements OnInit{
  separatorKeysCodes: number[] = [ENTER, COMMA];
  filteredGenres$!: Observable<string[]>;
  allGenres!: string[];
  genres: string[] = <string[]>[];
  movieForm!: FormGroup;
  httpErrors = false;
  errors: any;

  @ViewChild('genreInput') genreInput!: ElementRef<HTMLInputElement>;

  constructor(private movieService: MovieService, private genreService: GenreService, private router: Router) { }

  ngOnInit(): void {
    this.movieForm = new FormGroup({
      title: new FormControl('', [Validators.required]),
      description: new FormControl(''),
      ReleaseYear: new FormControl(new Date().getFullYear()),
      Duration: new FormControl('00:00'),
      Genres: new FormControl('')
    });
    
    this.genreService.getAll()
      .pipe(map(res => res.map(v => v.name)))
      .subscribe(res => {
        this.allGenres = res;
        this.filteredGenres$ = this.Genres!.valueChanges.pipe(
          startWith(null),
          map((genre: string | null) => (genre ? this._filter(genre) : this.allGenres.slice())),
        );  
      });
  }
  
  saveMovie(){
    if (this.movieForm.valid) {
      const title = this.title?.value;
      const description = this.description?.value;
      const ReleaseYear = this.ReleaseYear?.value;
      const Duration = this.Duration?.value;

      var newMovie = <Movie>({
        title: title,
        description: description,
        releaseYear: ReleaseYear,
        duration: Duration,
        genres: this.genres.map(val => {
          var g = new Genre();
          g.name = val;
          return g;
        })
      });

      this.movieService.addMovie(newMovie).subscribe({
        next: response => {
          this.router.navigate(['profile']);
        },
        error: error => {
          this.httpErrors = true;
          this.errors = error;
        }
      });
    }
  }

  remove(genre: string): void {
    const index = this.genres.indexOf(genre);

    if (index >= 0) {
      this.genres.splice(index, 1);
    }
  }
  
  add(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();

    if (value) {
      this.genres.push(value);
    }

    event.chipInput!.clear();

    this.Genres?.setValue(null);
  }
  
  selected(event: MatAutocompleteSelectedEvent): void {
    this.genres.push(event.option.viewValue);
    this.genreInput.nativeElement.value = '';
    this.Genres!.setValue(null);
  }
  
  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.allGenres.filter(genre => genre.toLowerCase().includes(filterValue));
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
