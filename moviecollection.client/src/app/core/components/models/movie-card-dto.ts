import { Genre } from "../../models/genre";

export class MovieCardDto {
  id!: string;
  title!: string;
  description!: string;
  releaseYear!: number;
  duration!: string;
  genres!: Genre[];
  onUserCollection!: boolean;
}