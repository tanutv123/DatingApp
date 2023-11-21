import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {getPaginatedResult, getPaginationHeaders} from "./pagination-helper.service";
import {Message} from "../_model/message.model";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {User} from "../_model/user.model";
import {BehaviorSubject, take} from "rxjs";
import {BusyService} from "./busy.service";

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private messageHubConnection: HubConnection | undefined;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) { }

  createHubConnection(user : User, otherUsername: string) {
    this.busyService.busy();
    this.messageHubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.messageHubConnection.start()
      .catch(error => console.log(error))
      .finally(() => {
        this.busyService.idle();
      });

    this.messageHubConnection.on('ReceiveMessageThread', messages => {
      console.log(messages);
      this.messageThreadSource.next(messages);
    });

    this.messageHubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([...messages, message]);
          console.log([...messages, message]);
        }
      })
    })

  }

  stopHubConnection() {
    if (this.messageHubConnection) {
      this.messageThreadSource.next([]);
      this.messageHubConnection.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    return this.messageHubConnection?.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
