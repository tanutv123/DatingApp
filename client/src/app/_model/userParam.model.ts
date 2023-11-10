import {User} from "./user.model";

export class UserParam {
  gender: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  pageSize = 5;
  orderBy = 'lastActive';
  constructor(user: User) {
    this.gender = user.gender === 'female' ? 'male' : 'female';
  }
}
