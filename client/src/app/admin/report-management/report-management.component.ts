import {Component, OnInit} from '@angular/core';
import {Report} from "../../_model/report.model";
import {AdminService} from "../../_services/admin.service";
import {BsModalService} from "ngx-bootstrap/modal";
import {ReportDetailsComponent} from "../../modals/report-details/report-details.component";

@Component({
  selector: 'app-report-management',
  templateUrl: './report-management.component.html',
  styleUrls: ['./report-management.component.css']
})
export class ReportManagementComponent implements OnInit{
  reports: Report[] = [];

  constructor(private adminService: AdminService,
              private modalService: BsModalService) {
  }

  ngOnInit() {
    this.getReports();
  }

  getReports() {
    this.adminService.getUserReports().subscribe({
      next: reports => {
        this.reports = reports;
      }
    })
  }

  openReportDetailsModal(report: Report) {
    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        content: report.content,
        reported: report.reportedUser,
        userReport: report.reports
      }
    };
    this.modalService.show(ReportDetailsComponent, config);
  }
}
