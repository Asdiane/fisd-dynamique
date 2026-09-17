import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminSlide, Slide } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/slides`;

@Injectable({ providedIn: 'root' })
export class SlidesService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Slide[]> {
    return this.http.get<Slide[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminSlide[]> {
    return this.http.get<AdminSlide[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminSlide | undefined> {
    return this.getAll().pipe(map((slides) => slides.find((s) => s.id === id)));
  }

  create(slide: Omit<AdminSlide, 'id'>): Observable<AdminSlide> {
    return this.http.post<AdminSlide>(BASE_URL, slide);
  }

  update(id: string, slide: Omit<AdminSlide, 'id'>): Observable<AdminSlide> {
    return this.http.put<AdminSlide>(`${BASE_URL}/${id}`, slide);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
