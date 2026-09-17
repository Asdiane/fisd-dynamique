import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Souvenir, SouvenirPhoto } from './content.models';

const BASE_URL = `${environment.apiUrl}/api/souvenirs`;

@Injectable({ providedIn: 'root' })
export class SouvenirsService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Souvenir[]> {
    return this.http.get<Souvenir[]>(BASE_URL);
  }

  getById(id: string): Observable<Souvenir | undefined> {
    return this.getAll().pipe(map((souvenirs) => souvenirs.find((s) => s.id === id)));
  }

  getPhotos(souvenirId: string): Observable<SouvenirPhoto[]> {
    return this.http.get<SouvenirPhoto[]>(`${BASE_URL}/${souvenirId}/photos`);
  }

  create(souvenir: Omit<Souvenir, 'id'>): Observable<Souvenir> {
    return this.http.post<Souvenir>(BASE_URL, souvenir);
  }

  update(id: string, souvenir: Omit<Souvenir, 'id'>): Observable<Souvenir> {
    return this.http.put<Souvenir>(`${BASE_URL}/${id}`, souvenir);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }

  addPhoto(souvenirId: string, photo: Omit<SouvenirPhoto, 'id' | 'souvenirId'>): Observable<SouvenirPhoto> {
    return this.http.post<SouvenirPhoto>(`${BASE_URL}/${souvenirId}/photos`, photo);
  }

  deletePhoto(souvenirId: string, photoId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${souvenirId}/photos/${photoId}`);
  }
}
