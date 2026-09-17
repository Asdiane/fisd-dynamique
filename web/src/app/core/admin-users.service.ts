import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminRole, AdminUser } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/admin-users`;

export interface InviteAdminUserPayload {
  email: string;
  role: AdminRole;
}

export interface PendingAdminInvitation {
  id: string;
  email: string;
  role: AdminRole;
  expiresAt: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class AdminUsersService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(BASE_URL);
  }

  invite(payload: InviteAdminUserPayload): Observable<void> {
    return this.http.post<void>(`${BASE_URL}/invite`, payload);
  }

  getPendingInvitations(): Observable<PendingAdminInvitation[]> {
    return this.http.get<PendingAdminInvitation[]>(`${BASE_URL}/invitations`);
  }

  cancelInvitation(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/invitations/${id}`);
  }

  updateRole(id: string, role: AdminRole): Observable<AdminUser> {
    return this.http.put<AdminUser>(`${BASE_URL}/${id}/role`, { role });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }

  setActive(id: string, isActive: boolean): Observable<AdminUser> {
    return this.http.put<AdminUser>(`${BASE_URL}/${id}/status`, { isActive });
  }

  resetTwoFactor(id: string): Observable<AdminUser> {
    return this.http.post<AdminUser>(`${BASE_URL}/${id}/reset-2fa`, {});
  }
}
