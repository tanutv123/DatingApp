import {Component, OnInit} from '@angular/core';
import {Member} from "../../_model/member.model";
import {MemberService} from "../../_services/member.service";
import {ActivatedRoute} from "@angular/router";
import {CommonModule} from "@angular/common";
import {TabsModule} from "ngx-bootstrap/tabs";
import {GalleryItem, GalleryModule, ImageItem} from "ng-gallery";

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule]
})
export class MemberDetailComponent implements OnInit{
  member: Member | undefined;
  images: GalleryItem[] = [];
  constructor(private memberService: MemberService, private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    const userName = this.route.snapshot.paramMap.get('username');
    if (userName) this.memberService.getMember(userName).subscribe({
      next: member => {
        this.member = member;
        this.getImages();
      }
    });
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    }
  }

}
