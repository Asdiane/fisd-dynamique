import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../../environments/environment';
export interface EditorialButton {
  labelFr: string;
  labelEn: string;
  path: string;
}
export interface HomeSection {
  id: string;
  kind: string;
  visible: boolean;
  titleFr: string;
  titleEn: string;
  textFr: string;
  textEn: string;
  buttons: EditorialButton[];
}
export interface EditorialStat {
  value: number;
  suffix: string;
  labelFr: string;
  labelEn: string;
}
export interface EditorialContent {
  fr: Record<string, string>;
  en: Record<string, string>;
  sections: HomeSection[];
  stats: EditorialStat[];
  themeImage: string;
  reportUrl: string;
  newsletterUrl: string;
  tikTokUrl: string;
  youTubeUrl: string;
}
export function defaultEditorial(): EditorialContent {
  return {
    fr: {},
    en: {},
    sections: ['theme', 'program', 'speakers', 'stats', 'pillars', 'testimonials', 'partners'].map(
      (kind) => ({
        id: kind,
        kind,
        visible: true,
        titleFr: '',
        titleEn: '',
        textFr: '',
        textEn: '',
        buttons: [],
      }),
    ),
    stats: [{ value: 3, suffix: '', labelFr: 'Éditions', labelEn: 'Editions' }],
    themeImage: 'assets/images/concertations.jpeg',
    reportUrl: '',
    newsletterUrl: '',
    tikTokUrl: '',
    youTubeUrl: '',
  };
}
@Injectable({ providedIn: 'root' })
export class EditorialService {
  readonly content = signal<EditorialContent>(defaultEditorial());
  private originals: Record<string, Record<string, string>> = { fr: {}, en: {} };
  readonly url = environment.apiUrl + '/api/editorial';
  constructor(
    private http: HttpClient,
    private translate: TranslateService,
  ) {}
  get() {
    return this.http.get<EditorialContent | null>(this.url);
  }
  save(value: EditorialContent) {
    return this.http.put<EditorialContent>(this.url, value);
  }
  apply(value: EditorialContent | null) {
    this.content.set(value ?? defaultEditorial());
    for (const lang of ['fr', 'en'] as const) {
      const nested: Record<string, Record<string, string>> = {};
      for (const [key, text] of Object.entries(this.originals[lang])) {
        const dot = key.indexOf('.');
        (nested[key.slice(0, dot)] ??= {})[key.slice(dot + 1)] = text;
      }
      if (lang === this.translate.getCurrentLang())
        for (const key of Object.keys(this.content()[lang])) {
          if (!(key in this.originals[lang]))
            this.originals[lang][key] = this.translate.instant(key);
        }
      for (const [key, text] of Object.entries(this.content()[lang])) {
        const dot = key.indexOf('.');
        if (dot < 1) continue;
        const group = key.slice(0, dot);
        if (['__proto__', 'constructor', 'prototype'].includes(group)) continue;
        (nested[group] ??= {})[key.slice(dot + 1)] = text;
      }
      this.translate.setTranslation(lang, nested, true);
    }
  }
  local(fr: string, en: string) {
    return this.translate.getCurrentLang() === 'en' ? en : fr;
  }
}
