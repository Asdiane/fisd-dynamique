import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminEdition, Edition, ProgramDay, ScheduleItem } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/editions`;

@Injectable({ providedIn: 'root' })
export class EditionsService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<AdminEdition[]> {
    return this.http.get<AdminEdition[]>(BASE_URL);
  }

  getVisible(): Observable<Edition[]> {
    return this.http.get<Edition[]>(`${BASE_URL}/visible`);
  }

  getById(id: string): Observable<AdminEdition | undefined> {
    return this.getAll().pipe(map((editions) => editions.find((e) => e.id === id)));
  }

  create(edition: Omit<AdminEdition, 'id' | 'isCurrent'>): Observable<AdminEdition> {
    return this.http.post<AdminEdition>(BASE_URL, edition);
  }

  update(id: string, edition: Omit<AdminEdition, 'id' | 'isCurrent'>): Observable<AdminEdition> {
    return this.http.put<AdminEdition>(`${BASE_URL}/${id}`, edition);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }

  setCurrent(id: string): Observable<AdminEdition> {
    return this.http.put<AdminEdition>(`${BASE_URL}/${id}/current`, {});
  }

  getDays(editionId: string): Observable<ProgramDay[]> {
    return this.http.get<ProgramDay[]>(`${BASE_URL}/${editionId}/days`);
  }

  addDay(editionId: string, day: Omit<ProgramDay, 'id' | 'editionId'>): Observable<ProgramDay> {
    return this.http.post<ProgramDay>(`${BASE_URL}/${editionId}/days`, day);
  }

  updateDay(editionId: string, dayId: string, day: Omit<ProgramDay, 'id' | 'editionId'>): Observable<ProgramDay> {
    return this.http.put<ProgramDay>(`${BASE_URL}/${editionId}/days/${dayId}`, day);
  }

  deleteDay(editionId: string, dayId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${editionId}/days/${dayId}`);
  }

  getSchedule(dayId: string): Observable<ScheduleItem[]> {
    return this.http.get<ScheduleItem[]>(`${BASE_URL}/days/${dayId}/schedule`);
  }

  addScheduleItem(dayId: string, item: Omit<ScheduleItem, 'id' | 'programDayId'>): Observable<ScheduleItem> {
    return this.http.post<ScheduleItem>(`${BASE_URL}/days/${dayId}/schedule`, item);
  }

  updateScheduleItem(dayId: string, itemId: string, item: Omit<ScheduleItem, 'id' | 'programDayId'>): Observable<ScheduleItem> {
    return this.http.put<ScheduleItem>(`${BASE_URL}/days/${dayId}/schedule/${itemId}`, item);
  }

  deleteScheduleItem(dayId: string, itemId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/days/${dayId}/schedule/${itemId}`);
  }

  // The edition an admin has explicitly marked current (AdminEditions > "Définir comme
  // courante") - this is what the homepage and ticketing link resolve against now, instead of
  // a client-side "soonest unfinished, else latest year" guess that had no admin control at all.
  getCurrentEdition(): Observable<Edition | null> {
    return this.http.get<Edition>(`${BASE_URL}/current`).pipe(catchError(() => of(null)));
  }

  resolveCurrentTicketingUrl(fallback: string): Observable<string> {
    return this.getCurrentEdition().pipe(map((edition) => edition?.ticketingUrl || fallback));
  }
}
