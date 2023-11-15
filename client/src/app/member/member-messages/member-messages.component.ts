import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {Message} from "../../_model/message.model";
import {MessageService} from "../../_services/message.service";
import {CommonModule} from "@angular/common";
import {TimeagoModule} from "ngx-timeago";
import {FormsModule, NgForm} from "@angular/forms";
import {take} from "rxjs";

@Component({
  standalone: true,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit{
  @Input() username: string | undefined;
  @ViewChild('messageForm') messageForm: NgForm | undefined;
  messageContent = '';
  constructor(public messageService: MessageService) {
  }

  ngOnInit() {
  }

  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    });
  }
}
