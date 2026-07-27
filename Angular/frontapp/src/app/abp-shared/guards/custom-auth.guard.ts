import { AuthService } from '@abp/ng.core';
import { inject } from '@angular/core';
import type { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, delay, of, tap } from 'rxjs';


export const customAuthGuard: CanActivateFn = (
  childRoute: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean> | boolean | UrlTree => {
  let authService = inject(AuthService);
  if (authService.isAuthenticated) {
    return true;
  }

  authService.navigateToLogin();
  return false;
};
