import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { LocaleService } from './locale.service';

// Attaches the visitor's current language to every API call so content endpoints (slides,
// pillars, editions, ...) can resolve their Fr/En fields server-side without every service
// having to thread a lang param through by hand.
export const localeInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  const lang = inject(LocaleService).currentLang();
  return next(req.clone({ setHeaders: { 'Accept-Language': lang } }));
};
