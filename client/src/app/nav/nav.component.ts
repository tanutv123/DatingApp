import {Component, OnInit} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {Observable, of} from "rxjs";
import {User} from "../_model/user.model";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model: any = {};
  loggedIn = false;
  constructor(public accountService: AccountService,
              private router: Router) {
  }

  ngOnInit() {
  }



  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => this.router.navigateByUrl('/members')
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  loginAsTodd() {
    this.accountService.loginAsTodd().subscribe({
      next: _ => this.router.navigateByUrl('/members'),
    });
  }


}
