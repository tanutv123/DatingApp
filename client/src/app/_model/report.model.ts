import {Member} from "./member.model";
import {UserReport} from "./userReport.model";

export interface Report {
  id: string;
  content: string;
  createdDate: string;
  reporter: Member;
  reportedUser: Member;
  reports: UserReport[];
}
