import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Article } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/articles`;

@Injectable({ providedIn: 'root' })
export class ArticlesService {
  constructor(private http: HttpClient) {}

  getPublished(): Observable<Article[]> {
    return this.http.get<Article[]>(`${BASE_URL}/published`);
  }

  getById(id: string): Observable<Article> {
    return this.http.get<Article>(`${BASE_URL}/${id}`);
  }

  getAll(): Observable<Article[]> {
    return this.http.get<Article[]>(BASE_URL);
  }

  create(article: Pick<Article, 'title' | 'excerpt' | 'content' | 'imageUrl' | 'status'>): Observable<Article> {
    return this.http.post<Article>(BASE_URL, { ...article, publishedAt: null });
  }

  // publishedAt is only ever set the first time an article is published, and is never cleared
  // afterward (even if it's saved back to draft) - so a republish keeps its original publish date.
  update(
    id: string,
    article: Pick<Article, 'title' | 'excerpt' | 'content' | 'imageUrl' | 'status' | 'publishedAt'>
  ): Observable<Article> {
    return this.http.put<Article>(`${BASE_URL}/${id}`, article);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
