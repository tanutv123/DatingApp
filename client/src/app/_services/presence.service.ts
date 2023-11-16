import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {ToastrService} from "ngx-toastr";
import {User} from "../_model/user.model";
import {BehaviorSubject, take} from "rxjs";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection : HubConnection | undefined;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => {
          this.onlineUserSource.next([...usernames, username]);
        }
      })
    });
    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => {
          this.onlineUserSource.next(usernames.filter(x => x != username));
        }
      })
    });

    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUserSource.next(usernames);
    });
    this.hubConnection.on('NewMessageReceived', sender => {
      this.toastr
        .info(sender.knownAs + ' has sent you a new message! Click me to see the message')
        .onTap
        .pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('/members/' + sender.username + '?tab=Messages')
        })
    })
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
