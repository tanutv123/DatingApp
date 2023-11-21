import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {User} from "../_model/user.model";
import {Report} from "../_model/report.model";
import {ReportType} from "../_model/reportType.model";

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUserWithRoles() {
    return this.http.get<User[]>(this.baseUrl + "admin/users-with-roles");
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username
                                        + '?roles=' + roles, {});
  }

  getUserReports() {
    return this.http.get<Report[]>(this.baseUrl + 'report');
  }

  restrictUser(username: string) {
    return this.http.put(this.baseUrl + 'admin/restrict-user/' + username, {});
  }

}
