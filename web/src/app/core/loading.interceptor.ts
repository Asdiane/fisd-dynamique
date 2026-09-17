import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { finalize } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoadingService } from './loading.service';

// Opt-in: only a request that explicitly carries a `loaderId` header drives the global
// overlay. A request with no loaderId (auth checks, background polling, etc.) passes
// through untouched - no flicker, no spinner.
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  const loaderId = req.headers.get('loaderId') ?? '';
  if (!loaderId) {
    return next(req);
  }

  const loading = inject(LoadingService);
  loading.setLoading(loaderId, true);
  return next(req).pipe(finalize(() => loading.setLoading(loaderId, false)));
};
