import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Member} from "../_model/member.model";
import {map, of, take} from "rxjs";
import {PaginatedResult} from "../_model/pagination.model";
import {UserParam} from "../_model/userParam.model";
import {AccountService} from "./account.service";
import {User} from "../_model/user.model";
import {UserLikeParam} from "../_model/userlikeParam.model";
import {getPaginatedResult, getPaginationHeaders} from "./pagination-helper.service";

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //Use key-value pairs to cache the query string and the result
  //Key: query string
  //Values: paginated result
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParam | undefined;
  userLikeParams: UserLikeParam | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParam(user);
          this.user = user;
        }
      }
    });
    this.userLikeParams = new UserLikeParam();
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParam) {
    this.userParams = params;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParam(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParam: UserParam) {
    //Getting the query values from the cache
    const response = this.memberCache.get(Object.values(userParam).join('-'));
    //Return an observable with response value if it is contained in the cache
    //If it is not contained in the cache, it will send a request to the server
    if (response) return of(response);
    let params = getPaginationHeaders(userParam.pageNumber, userParam.pageSize);
    params = params.append('minAge', userParam.minAge);
    params = params.append('maxAge', userParam.maxAge);
    params = params.append('gender', userParam.gender);
    params = params.append('orderBy', userParam.orderBy);
    //Use map() method in this GET method in order to set a key-value pairs for our cache value
    return getPaginatedResult<Member[]>(this.baseUrl + "users", params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParam).join('-'), response);
        return response;
      })
    );
  }

  getMember(username: string) {
    //This member array will have duplicated user though
    //This is still more efficient than sending a request to the api
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), []).find((member: Member) => member.userName == username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + "users/" + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(_ => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + "likes/" + username, {});
  }

  getLikes(userLikeParam: UserLikeParam) {
    const response = this.memberCache.get(Object.values(userLikeParam).join('-'));
    if (response) return of(response);
    let params = getPaginationHeaders(userLikeParam.pageNumber, userLikeParam.pageSize);
     params = params.append('predicate', userLikeParam.predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http).pipe(
      map(response => {
        if (response.result) {
          this.memberCache.set(Object.values(userLikeParam).join('-'), response);
        }
        return response;
      })
    );
  }

  setUserLikeParam(userLikeParam: UserLikeParam) {
    this.userLikeParams = userLikeParam;
  }

  getUserLikeParam() {
    return this.userLikeParams;
  }

}
