import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminPillar, Pillar } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/pillars`;

@Injectable({ providedIn: 'root' })
export class PillarsService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Pillar[]> {
    return this.http.get<Pillar[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminPillar[]> {
    return this.http.get<AdminPillar[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminPillar | undefined> {
    return this.getAll().pipe(map((pillars) => pillars.find((p) => p.id === id)));
  }

  create(pillar: Omit<AdminPillar, 'id'>): Observable<AdminPillar> {
    return this.http.post<AdminPillar>(BASE_URL, pillar);
  }

  update(id: string, pillar: Omit<AdminPillar, 'id'>): Observable<AdminPillar> {
    return this.http.put<AdminPillar>(`${BASE_URL}/${id}`, pillar);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
