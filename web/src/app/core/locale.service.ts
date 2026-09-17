import { Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { EditorialService } from './editorial.service';

const LANG_KEY = 'fisd_lang';

@Injectable({ providedIn: 'root' })
export class LocaleService {
  currentLang = signal<'fr' | 'en'>('fr');

  constructor(
    private translate: TranslateService,
    private editorial: EditorialService,
  ) {
    const saved = (localStorage.getItem(LANG_KEY) as 'fr' | 'en') || 'fr';
    this.currentLang.set(saved);
    this.translate
      .use(saved)
      .subscribe(() =>
        this.editorial
          .get()
          .subscribe({ next: (value) => this.editorial.apply(value), error: () => undefined }),
      );
    document.documentElement.lang = saved;
  }

  // A full reload (not just translate.use()) is deliberate: bilingual content coming from the
  // API is already resolved server-side to one language when it's first fetched (see
  // localeInterceptor), so simply switching the ngx-translate lang would leave every
  // already-loaded piece of content stuck in the old language. Reloading re-runs every request
  // with the new Accept-Language header, which is the only way to keep chrome text and content
  // in sync everywhere at once.
  setLocale(lang: 'fr' | 'en'): void {
    if (this.currentLang() === lang) {
      return;
    }
    localStorage.setItem(LANG_KEY, lang);
    window.location.reload();
  }

  toggle(): void {
    const next = this.currentLang() === 'fr' ? 'en' : 'fr';
    this.setLocale(next);
  }
}
