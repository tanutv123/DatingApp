import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {CreateReport} from "../_model/createReport.model";
import {HttpClient} from "@angular/common/http";
import {ReportType} from "../_model/reportType.model";

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  addReport(createReport: CreateReport) {
    return this.http.post(this.baseUrl + 'report', createReport);
  }
  getReportTypes() {
    return this.http.get<ReportType[]>(this.baseUrl + 'report/report-types');
  }
}
