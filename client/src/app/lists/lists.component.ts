import {Component, OnInit} from '@angular/core';
import {Member} from "../_model/member.model";
import {MemberService} from "../_services/member.service";
import {Pagination} from "../_model/pagination.model";
import {UserLikeParam} from "../_model/userlikeParam.model";

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit{
  members: Member[] | undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;
  userLikeParam: UserLikeParam | undefined;

  constructor(private memberService: MemberService) {
    this.userLikeParam = this.memberService.userLikeParams;
  }

  ngOnInit() {
    console.log(this.userLikeParam);
    this.loadLikes();
  }

  loadLikes() {
    if (!this.userLikeParam) return;
    this.memberService.setUserLikeParam(this.userLikeParam);
    this.memberService.getLikes(this.userLikeParam).subscribe({
      next: response => {
        console.log(response?.result);
        this.members = response.result;
        this.pagination = response.pagination;
      }
    })
  }

  pageChanged(event : any) {
    if (this.userLikeParam && this.userLikeParam?.pageNumber != event.page) {
      this.userLikeParam.pageNumber = event.page;
      this.memberService.setUserLikeParam(this.userLikeParam);
      this.loadLikes();
    }
  }

}
