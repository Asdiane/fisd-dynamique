import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AUTH_TOKEN_KEY } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  const token = sessionStorage.getItem(AUTH_TOKEN_KEY);
  if (!token) {
    return next(req);
  }

  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
