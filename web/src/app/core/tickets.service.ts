import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Ticket, TicketAttachment } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/tickets`;

@Injectable({ providedIn: 'root' })
export class TicketsService {
  constructor(private http: HttpClient) {}

  getMine(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${BASE_URL}/mine`);
  }

  getAll(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(BASE_URL);
  }

  create(ticket: { category: string; title: string; description: string }): Observable<Ticket> {
    return this.http.post<Ticket>(BASE_URL, ticket);
  }

  updateStatus(id: string, status: string, resolutionNote: string): Observable<Ticket> {
    return this.http.put<Ticket>(`${BASE_URL}/${id}/status`, { status, resolutionNote });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }

  uploadAttachment(ticketId: string, file: File): Observable<TicketAttachment> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<TicketAttachment>(`${BASE_URL}/${ticketId}/attachments`, formData);
  }

  deleteAttachment(ticketId: string, attachmentId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${ticketId}/attachments/${attachmentId}`);
  }
}
