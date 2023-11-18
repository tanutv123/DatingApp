import {Component, ViewChild} from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";
import {Member} from "../../_model/member.model";
import {environment} from "../../../environments/environment";
import {Report} from "../../_model/report.model";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-report-modal',
  templateUrl: './report-modal.component.html',
  styleUrls: ['./report-modal.component.css']
})
export class ReportModalComponent {
  @ViewChild('reportForm') reportForm: NgForm | undefined;
  member: Member | undefined;
  baseUrl = environment.apiUrl;
  report: Report = {} as Report;
  options: Object = {
    charCounterMax: 5,
    imageUploadMethod: 'POST',
    imageUploadURL: this.baseUrl + 'images',
    events: {
      'image.beforeUpload': (images: any) => {
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
  constructor(public bsModalRef: BsModalRef) {
  }

  reportSubmit() {
    console.log(this.reportForm?.value);
  }
}
