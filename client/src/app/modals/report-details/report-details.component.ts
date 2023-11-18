import {Component, OnInit} from '@angular/core';
import {Member} from "../../_model/member.model";
import {BsModalRef} from "ngx-bootstrap/modal";
import {AdminService} from "../../_services/admin.service";
import {UserReport} from "../../_model/userReport.model";

@Component({
  selector: 'app-report-details',
  templateUrl: './report-details.component.html',
  styleUrls: ['./report-details.component.css']
})
export class ReportDetailsComponent{
  content= '';
  reported: Member = {} as Member;
  restrictedClicked = false;
  userReport: UserReport[] = [];

  constructor(public bsModalRef : BsModalRef, private adminService: AdminService) {
  }

  restrictMember() {
    this.adminService.restrictUser(this.reported.userName).subscribe({
      next: _ => this.restrictedClicked = true
    });
  }

}
