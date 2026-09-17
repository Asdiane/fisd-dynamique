import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const BASE_URL = `${environment.apiUrl}/api/audit-logs`;

export interface AuditLog {
  id: number;
  timestamp: string;
  adminUserEmail: string | null;
  tableName: string;
  action: string;
  keyValues: string | null;
}

export interface AuditLogDetail extends AuditLog {
  oldValues: string | null;
  newValues: string | null;
}

export interface AuditLogPage {
  items: AuditLog[];
  totalCount: number;
}

export interface AuditLogQuery {
  tableName?: string;
  action?: string;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class AuditLogsService {
  constructor(private http: HttpClient) {}

  getAll(query: AuditLogQuery): Observable<AuditLogPage> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.tableName) {
      params = params.set('tableName', query.tableName);
    }
    if (query.action) {
      params = params.set('action', query.action);
    }
    return this.http.get<AuditLogPage>(BASE_URL, { params });
  }

  getDetail(id: number): Observable<AuditLogDetail> {
    return this.http.get<AuditLogDetail>(`${BASE_URL}/${id}`);
  }
}
