import { Genre } from "./genre";

export class Movie {
  id!: string;
  title!: string;
  description!: string;
  releaseYear!: number;
  duration!: string;
  genres!: Genre[];
}