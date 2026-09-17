import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from './auth.service';
import { AdminRole } from './content.models';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.checkAuth().pipe(map((isEditor) => (isEditor ? true : router.parseUrl('/admin'))));
};

export const superAdminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.checkAuth().pipe(
    map((isEditor) => {
      if (!isEditor) return router.parseUrl('/admin');
      const role = authService.role();
      return role === AdminRole.SuperAdmin || role === AdminRole.PlatformAdmin ? true : router.parseUrl('/admin/articles');
    })
  );
};

export const platformAdminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.checkAuth().pipe(
    map((isEditor) => {
      if (!isEditor) return router.parseUrl('/admin');
      return authService.role() === AdminRole.PlatformAdmin ? true : router.parseUrl('/admin/articles');
    })
  );
};
