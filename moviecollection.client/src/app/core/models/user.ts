import { Movie } from "./movie";

export class User {
  id!: string;
  email!: string;
  firstName!: string;
  lastName!: string;
  enabled!: boolean;
  hasIncompletedInformation!: boolean;
  emailConfirmed!: boolean;
  twoFactorEnabled!: boolean;
  movieCollection!: Movie[];
}
