import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Member} from "../../_model/member.model";
import {ActivatedRoute} from "@angular/router";
import {CommonModule} from "@angular/common";
import {TabDirective, TabsetComponent, TabsModule} from "ngx-bootstrap/tabs";
import {GalleryItem, GalleryModule, ImageItem} from "ng-gallery";
import {TimeagoModule} from "ngx-timeago";
import {MemberMessagesComponent} from "../member-messages/member-messages.component";
import {MessageService} from "../../_services/message.service";
import {Message} from "../../_model/message.model";
import {PresenceService} from "../../_services/presence.service";
import {AccountService} from "../../_services/account.service";
import {User} from "../../_model/user.model";
import {take} from "rxjs";
import {BsModalRef, BsModalService, ModalModule} from "ngx-bootstrap/modal";
import {ReportModalComponent} from "../../modals/report-modal/report-modal.component";
import {AdminService} from "../../_services/admin.service";
import {ReportType} from "../../_model/reportType.model";
import {ReportService} from "../../_services/report.service";

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent, ModalModule]
})
export class MemberDetailComponent implements OnInit, OnDestroy{
  @ViewChild('memberTabs', {static: true}) memberTabs : TabsetComponent | undefined;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab: TabDirective | undefined;
  messages: Message[] = [];
  user: User | undefined;
  bsModalRef: BsModalRef<ReportModalComponent> = new BsModalRef<ReportModalComponent>();
  reportTypes: ReportType[] = [];
  constructor(private accountService: AccountService,
              private route: ActivatedRoute,
              private messageService: MessageService,
              public presenceService: PresenceService,
              private modalService: BsModalService,
              private reportService: ReportService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user;
      }
    })
  }

  ngOnInit() {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    });
    this.getImages();
    this.loadReportTypes();
  }

  ngOnDestroy() {
    this.messageService.stopHubConnection();
  }

  onTabActivate(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }

  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    }
  }

  openReportModal() {
    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        member: this.member,
        reportTypes: this.reportTypes
      }
    };
    this.bsModalRef = this.modalService.show(ReportModalComponent, config);
  }

  loadReportTypes() {
    this.reportService.getReportTypes().subscribe({
      next: reportTypes => this.reportTypes = reportTypes
    })
  }
}
