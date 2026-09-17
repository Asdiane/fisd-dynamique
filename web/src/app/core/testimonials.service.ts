import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminTestimonial, Testimonial } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/testimonials`;

@Injectable({ providedIn: 'root' })
export class TestimonialsService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Testimonial[]> {
    return this.http.get<Testimonial[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminTestimonial[]> {
    return this.http.get<AdminTestimonial[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminTestimonial | undefined> {
    return this.getAll().pipe(map((testimonials) => testimonials.find((t) => t.id === id)));
  }

  create(testimonial: Omit<AdminTestimonial, 'id'>): Observable<AdminTestimonial> {
    return this.http.post<AdminTestimonial>(BASE_URL, testimonial);
  }

  update(id: string, testimonial: Omit<AdminTestimonial, 'id'>): Observable<AdminTestimonial> {
    return this.http.put<AdminTestimonial>(`${BASE_URL}/${id}`, testimonial);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
