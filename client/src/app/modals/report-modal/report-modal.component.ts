import {Component, ViewChild} from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";
import {Member} from "../../_model/member.model";
import {environment} from "../../../environments/environment";
import {Report} from "../../_model/report.model";
import {NgForm} from "@angular/forms";
import {ReportType} from "../../_model/reportType.model";
import {CreateReport} from "../../_model/createReport.model";
import {ReportService} from "../../_services/report.service";

@Component({
  selector: 'app-report-modal',
  templateUrl: './report-modal.component.html',
  styleUrls: ['./report-modal.component.css']
})
export class ReportModalComponent {
  @ViewChild('reportForm') reportForm: NgForm | undefined;
  member: Member | undefined;
  baseUrl = environment.apiUrl;
  report: CreateReport = {} as CreateReport;
  reportTypes: ReportType[] = [];
  isAdded = false;
  options: Object = {
    charCounterMax: 200,
    imageUploadMethod: 'POST',
    imageUploadURL: this.baseUrl + 'images',
    events: {
      'image.beforeUpload': () => {
        console.log('before upload');
      },
      'image.uploaded': () => {
        console.log('uploaded');
      },
      'image.error': (error: any, response: any) => {
        console.log(error);
        // Response contains the original server response to the request if available.
      }
    }
  }
  constructor(public bsModalRef: BsModalRef, private reportService: ReportService) {
    this.report.reportTypes = [];
  }



  reportSubmit() {
    if (!this.member) return;
    this.report.reportedUsername = this.member?.userName;
    this.reportService.addReport(this.report).subscribe({
      next: _ => this.isAdded = true
    });
  }

  updateCheckedReportTypes(type: ReportType) {
    if (!this.report.reportTypes) this.report.reportTypes = [];
    const index = this.report.reportTypes.indexOf(type);
    index !== -1 ? this.report.reportTypes.splice(index, 1) : this.report.reportTypes.push(type);
  }
}
