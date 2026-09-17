import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Contact } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/contacts`;

@Injectable({ providedIn: 'root' })
export class ContactsService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<Contact[]> {
    return this.http.get<Contact[]>(BASE_URL);
  }

  getById(id: string): Observable<Contact | undefined> {
    return this.getAll().pipe(map((contacts) => contacts.find((c) => c.id === id)));
  }

  create(contact: Omit<Contact, 'id'>): Observable<Contact> {
    return this.http.post<Contact>(BASE_URL, contact);
  }

  update(id: string, contact: Omit<Contact, 'id'>): Observable<Contact> {
    return this.http.put<Contact>(`${BASE_URL}/${id}`, contact);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
