import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const BASE_URL = `${environment.apiUrl}/api/media`;

export interface MediaFile {
  id: string;
  category: string;
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
  url: string;
  altText: string | null;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class MediaService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<MediaFile[]> {
    return this.http.get<MediaFile[]>(BASE_URL);
  }

  upload(category: string, file: File, altText?: string): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);
    if (altText) formData.append('altText', altText);
    return this.http.post<MediaFile>(`${BASE_URL}/upload/${category}`, formData);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${id}`);
  }
}
