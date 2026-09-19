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
export interface FooterItem { group: string; textFr: string; textEn: string; url: string; visible: boolean; }
export const DEFAULT_FOOTER_ITEMS: FooterItem[] = [{"group":"about","textFr":"Présentation","textEn":"Presentation","url":"/#presentation","visible":true},{"group":"about","textFr":"Actualités","textEn":"News","url":"/actualites","visible":true},{"group":"about","textFr":"Éditions précédentes","textEn":"Previous editions","url":"/galerie","visible":true},{"group":"menu","textFr":"Conditions","textEn":"Terms","url":"/conditions","visible":true},{"group":"menu","textFr":"Devenir partenaire","textEn":"Become a partner","url":"/agir#partenaire","visible":true},{"group":"menu","textFr":"Devenir bénévole","textEn":"Volunteer","url":"/agir#benevole","visible":true},{"group":"menu","textFr":"Devenir exposant","textEn":"Become an exhibitor","url":"/agir#stand","visible":true}];
export interface EditorialContent {
  footerItems?: FooterItem[] | null;
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
    footerItems: structuredClone(DEFAULT_FOOTER_ITEMS),
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
    stats: [{"labelFr":"Partenaires","suffix":"","value":18,"labelEn":"Partners"},{"labelFr":"Axes d’engagement","suffix":"","value":3,"labelEn":"Areas of engagement"},{"labelFr":"Intervenants et panélistes","suffix":"","value":14,"labelEn":"Speakers and panelists"},{"labelFr":"Pays membres","suffix":"+","value":20,"labelEn":"Member countries"},{"labelFr":"Éditions","suffix":"","value":3,"labelEn":"Editions"},{"labelFr":"Entreprises","suffix":"+","value":50,"labelEn":"Businesses"}],
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
