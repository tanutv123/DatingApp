import {Component, OnInit, ViewChild} from '@angular/core';
import {Report} from "../../_model/report.model";
import {NgForm} from "@angular/forms";
import {environment} from "../../../environments/environment";
import {Member} from "../../_model/member.model";

@Component({
  selector: 'app-member-report',
  templateUrl: './member-report.component.html',
  styleUrls: ['./member-report.component.css']
})
export class MemberReportComponent implements OnInit{
  report: Report = {} as Report;
  memberToReport : Member = {} as Member;
  baseUrl = environment.apiUrl;
  options: Object = {
    charCounterMax: 5,
    imageUploadUrl: this.baseUrl + 'api/images',
    events: {
      'image.uploaded': () => {
        console.log('uploaded')
      }
    }
  }


  ngOnInit() {
  }

  @ViewChild('reportForm') reportForm: NgForm | undefined;
  reportSubmit() {
    console.log(this.reportForm?.value);
  }

}
