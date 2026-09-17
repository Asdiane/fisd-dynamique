import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminParticipant, Participant } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/participants`;

@Injectable({ providedIn: 'root' })
export class ParticipantsService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Participant[]> {
    return this.http.get<Participant[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminParticipant[]> {
    return this.http.get<AdminParticipant[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminParticipant | undefined> {
    return this.getAll().pipe(map((participants) => participants.find((p) => p.id === id)));
  }

  create(participant: Omit<AdminParticipant, 'id'>): Observable<AdminParticipant> {
    return this.http.post<AdminParticipant>(BASE_URL, participant);
  }

  update(id: string, participant: Omit<AdminParticipant, 'id'>): Observable<AdminParticipant> {
    return this.http.put<AdminParticipant>(`${BASE_URL}/${id}`, participant);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
