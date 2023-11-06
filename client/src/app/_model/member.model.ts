import {Photo} from "./photo.model";

export interface Member {
  userName: string;
  photoUrl: string;
  gender: string;
  age: number;
  dateOfBirth: string;
  knownAs: string;
  created: string;
  lastActive: string;
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  photos: Photo[];
}
