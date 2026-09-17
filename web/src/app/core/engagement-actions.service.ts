import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminEngagementAction, EngagementAction } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/engagement-actions`;

@Injectable({ providedIn: 'root' })
export class EngagementActionsService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<EngagementAction[]> {
    return this.http.get<EngagementAction[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminEngagementAction[]> {
    return this.http.get<AdminEngagementAction[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminEngagementAction | undefined> {
    return this.getAll().pipe(map((actions) => actions.find((a) => a.id === id)));
  }

  create(action: Omit<AdminEngagementAction, 'id'>): Observable<AdminEngagementAction> {
    return this.http.post<AdminEngagementAction>(BASE_URL, action);
  }

  update(id: string, action: Omit<AdminEngagementAction, 'id'>): Observable<AdminEngagementAction> {
    return this.http.put<AdminEngagementAction>(`${BASE_URL}/${id}`, action);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
