import { Injectable } from "@angular/core";
import { Observable, map } from "rxjs";
import { Movie } from "../models/movie";
import { HttpClient } from "@angular/common/http";
import { UserService } from "./user.service";

@Injectable({
    providedIn: 'root'
  })
export class MovieService {

  constructor(private http: HttpClient, private userService: UserService) { }

  public search(searchString: string): Observable<Movie[]> {
    return this.http.get<Movie[]>('/api/movies?title=' + searchString, {
      observe: 'body',
      responseType: 'json'
    });
  }

  public addMovie(movie: Movie): Observable<Movie> {
    return this.http.post<Movie>('/api/movies', movie, {
      observe: 'body',
      responseType: 'json'
    }).pipe(map(res => {
      this.userService.addMovieToCollection(res.id)
        .subscribe(res => this.userService.refreshUserInfo().subscribe());
      return res;
    }));
  }
}
