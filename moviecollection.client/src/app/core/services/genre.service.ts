import { Injectable } from "@angular/core";
import { Observable, map } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { Genre } from "../models/genre";

@Injectable({
    providedIn: 'root'
  })
export class GenreService {

  constructor(private http: HttpClient) { }

  public getAll(): Observable<Genre[]> {
    return this.http.get<Genre[]>('/api/genres', {
      observe: 'body',
      responseType: 'json'
    });
  }
}
