import {ReportType} from "./reportType.model";

export interface CreateReport {
  content: string;
  reportedUsername: string;
  reportTypes: ReportType[];
}
