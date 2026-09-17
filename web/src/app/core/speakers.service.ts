import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminSpeaker, Speaker } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/speakers`;

@Injectable({ providedIn: 'root' })
export class SpeakersService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Speaker[]> {
    return this.http.get<Speaker[]>(`${BASE_URL}/visible`);
  }

  getById(id: string): Observable<AdminSpeaker | undefined> {
    return this.getAll().pipe(map((speakers) => speakers.find((s) => s.id === id)));
  }

  getVisibleByEdition(editionId: string): Observable<Speaker[]> {
    return this.http.get<Speaker[]>(`${BASE_URL}/visible/edition/${editionId}`);
  }

  getAll(): Observable<AdminSpeaker[]> {
    return this.http.get<AdminSpeaker[]>(BASE_URL);
  }

  create(speaker: Omit<AdminSpeaker, 'id'>): Observable<AdminSpeaker> {
    return this.http.post<AdminSpeaker>(BASE_URL, speaker);
  }

  update(id: string, speaker: Omit<AdminSpeaker, 'id'>): Observable<AdminSpeaker> {
    return this.http.put<AdminSpeaker>(`${BASE_URL}/${id}`, speaker);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
