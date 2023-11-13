import { ResolveFn } from '@angular/router';
import {Member} from "../_model/member.model";
import {inject} from "@angular/core";
import {MemberService} from "../_services/member.service";

export const memberDetailResolver: ResolveFn<Member> = (route, state) => {
  const memberSerivce = inject(MemberService);

  return memberSerivce.getMember(route.paramMap.get('username')!);
};
