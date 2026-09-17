import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminHelpArticle, HelpArticle } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/help-articles`;

@Injectable({ providedIn: 'root' })
export class HelpArticlesService {
  constructor(private http: HttpClient) {}

  getVisible(): Observable<HelpArticle[]> {
    return this.http.get<HelpArticle[]>(`${BASE_URL}/visible`);
  }

  getAll(): Observable<AdminHelpArticle[]> {
    return this.http.get<AdminHelpArticle[]>(BASE_URL);
  }

  getById(id: string): Observable<AdminHelpArticle | undefined> {
    return this.getAll().pipe(map((articles) => articles.find((a) => a.id === id)));
  }

  create(article: Omit<AdminHelpArticle, 'id'>): Observable<AdminHelpArticle> {
    return this.http.post<AdminHelpArticle>(BASE_URL, article);
  }

  update(id: string, article: Omit<AdminHelpArticle, 'id'>): Observable<AdminHelpArticle> {
    return this.http.put<AdminHelpArticle>(`${BASE_URL}/${id}`, article);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
