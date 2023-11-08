import {Component, HostListener, OnInit, ViewChild} from '@angular/core';
import {Member} from "../../_model/member.model";
import {User} from "../../_model/user.model";
import {AccountService} from "../../_services/account.service";
import {MemberService} from "../../_services/member.service";
import {take} from "rxjs";
import {ToastrService} from "ngx-toastr";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit{
  member: Member | undefined;
  user: User | null = null;
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private accountService: AccountService, private memberService: MemberService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user

    })
  }

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    if (!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe({
      next: member => this.member = member
    });
  }

  updateMember() {
    if (!this.member) return;
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success("Update user successfully");
        this.editForm?.reset(this.member);
      }
    })
  }

}
