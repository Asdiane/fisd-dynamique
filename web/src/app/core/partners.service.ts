import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Partner } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/partners`;

@Injectable({ providedIn: 'root' })
export class PartnersService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Partner[]> {
    return this.http.get<Partner[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<Partner[]> {
    return this.http.get<Partner[]>(BASE_URL);
  }

  getById(id: string): Observable<Partner | undefined> {
    return this.getAll().pipe(map((partners) => partners.find((p) => p.id === id)));
  }

  create(partner: Omit<Partner, 'id'>): Observable<Partner> {
    return this.http.post<Partner>(BASE_URL, partner);
  }

  update(id: string, partner: Omit<Partner, 'id'>): Observable<Partner> {
    return this.http.put<Partner>(`${BASE_URL}/${id}`, partner);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
