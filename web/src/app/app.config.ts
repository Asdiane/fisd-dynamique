import { ApplicationConfig, APP_INITIALIZER, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { routes } from './app.routes';
import { LocaleService } from './core/locale.service';
import { authInterceptor } from './core/auth.interceptor';
import { localeInterceptor } from './core/locale.interceptor';
import { loadingInterceptor } from './core/loading.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideHttpClient(withInterceptors([authInterceptor, localeInterceptor, loadingInterceptor])),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled' })),
    provideTranslateService({
      lang: 'fr',
      fallbackLang: 'fr',
      loader: provideTranslateHttpLoader({ prefix: '/assets/i18n/', suffix: `.json?v=${Date.now()}` })
    }),
    {
      provide: APP_INITIALIZER,
      useFactory: (localeService: LocaleService) => () => localeService,
      deps: [LocaleService],
      multi: true
    }
  ]
};
