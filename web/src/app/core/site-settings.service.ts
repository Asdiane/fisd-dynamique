import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

const BASE_URL = `${environment.apiUrl}/api/site-settings`;

export interface SiteSettings {
  slideDurationSeconds: number;
}

@Injectable({ providedIn: 'root' })
export class SiteSettingsService {
  constructor(private http: HttpClient) {}

  get(): Observable<SiteSettings> {
    return this.http.get<SiteSettings>(BASE_URL);
  }

  update(settings: SiteSettings): Observable<SiteSettings> {
    return this.http.put<SiteSettings>(BASE_URL, settings);
  }
}
