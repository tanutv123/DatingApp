import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../_services/admin.service";
import {User} from "../../_model/user.model";
import {BsModalRef, BsModalService, ModalOptions} from "ngx-bootstrap/modal";
import {RolesModalComponent} from "../../modals/roles-modal/roles-modal.component";
import {initialState} from "ngx-bootstrap/timepicker/reducer/timepicker.reducer";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  //Should get those available roles by calling a request to the API server
  availableRoles = [
    'Admin',
    'Moderator',
    'Member'
  ];
  constructor(private adminService: AdminService, private modalService: BsModalService) {
  }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: users => {
        this.users = users;
        console.log(this.users);
      }
    })
  }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.userName,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles]
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if (!this.compareTwoArray(selectedRoles!, user.roles)) {
          this.adminService.updateUserRoles(user.userName, selectedRoles!).subscribe({
            next: roles => user.roles = roles
          });
        }
      }
    })
  }

  private compareTwoArray(arr1: any[], arr2: any[]) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
