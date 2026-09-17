import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const BASE_URL = `${environment.apiUrl}/api/email-history`;

export interface EmailHistoryEntry {
  id: number;
  subject: string;
  sentFrom: string;
  sentTo: string;
  emailSent: boolean;
  errorMessage: string | null;
  createdAt: string;
}

export interface EmailHistoryDetail extends EmailHistoryEntry {
  body: string;
}

export interface EmailHistoryPage {
  items: EmailHistoryEntry[];
  totalCount: number;
}

export interface EmailHistoryQuery {
  status?: string;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class EmailHistoryService {
  constructor(private http: HttpClient) {}

  getAll(query: EmailHistoryQuery): Observable<EmailHistoryPage> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.status) {
      params = params.set('status', query.status);
    }
    return this.http.get<EmailHistoryPage>(BASE_URL, { params });
  }

  getDetail(id: number): Observable<EmailHistoryDetail> {
    return this.http.get<EmailHistoryDetail>(`${BASE_URL}/${id}`);
  }
}
