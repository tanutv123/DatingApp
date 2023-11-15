import {Component, Input, OnInit} from '@angular/core';
import {Member} from "../../_model/member.model";
import {MemberService} from "../../_services/member.service";
import {ToastrService} from "ngx-toastr";
import {PresenceService} from "../../_services/presence.service";

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
  @Input() member : Member | undefined;

  constructor(private memberService: MemberService,
              private toastr: ToastrService,
              public presenceService: PresenceService) {
  }

  ngOnInit() {
  }

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: _ => this.toastr.success("Liked successfully")
    })
  }

}
