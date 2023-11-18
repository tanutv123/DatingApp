import { CanDeactivateFn } from '@angular/router';
import {MemberEditComponent} from "../member/member-edit/member-edit.component";
import {inject} from "@angular/core";
import {ConfirmService} from "../_services/confirm.service";

export const preventUnsavedChangesGuard:
  CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService = inject(ConfirmService);
  if (component.editForm?.dirty) {
    return confirmService.confirm();
  }
  return true;
};
