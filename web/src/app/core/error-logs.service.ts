import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const BASE_URL = `${environment.apiUrl}/api/error-logs`;

export interface ErrorLog {
  id: number;
  logged: string;
  level: string;
  message: string;
  logger: string | null;
  requestMethod: string | null;
  requestUrl: string | null;
  userEmail: string | null;
  hasException: boolean;
}

export interface ErrorLogDetail extends ErrorLog {
  exception: string | null;
}

export interface ErrorLogPage {
  items: ErrorLog[];
  totalCount: number;
}

export interface ErrorLogQuery {
  level?: string;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class ErrorLogsService {
  constructor(private http: HttpClient) {}

  getAll(query: ErrorLogQuery): Observable<ErrorLogPage> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.level) {
      params = params.set('level', query.level);
    }
    return this.http.get<ErrorLogPage>(BASE_URL, { params });
  }

  getDetail(id: number): Observable<ErrorLogDetail> {
    return this.http.get<ErrorLogDetail>(`${BASE_URL}/${id}`);
  }
}
