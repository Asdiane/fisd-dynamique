import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ErrorLog } from './error-logs.service';

export interface SystemHealth {
  databaseHealthy: boolean;
  errorCount24h: number;
  warningCount24h: number;
  activeAdminCount: number;
  inactiveAdminCount: number;
  pendingInvitationCount: number;
  recentErrors: ErrorLog[];
}

@Injectable({ providedIn: 'root' })
export class SystemHealthService {
  constructor(private http: HttpClient) {}

  get(): Observable<SystemHealth> {
    return this.http.get<SystemHealth>(`${environment.apiUrl}/api/system-health`);
  }
}
