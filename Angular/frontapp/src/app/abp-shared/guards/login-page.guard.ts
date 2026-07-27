import { AuthService } from '@abp/ng.core';
import { inject } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    CanActivateChildFn,
    CanActivateFn,
    Router,
    RouterStateSnapshot,
    UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';

export const loginPageAuthGuard: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
): Observable<boolean> | boolean | UrlTree => {
    let router = inject(Router);
    let authService = inject(AuthService);

    let { backoffice } = route.data || {};

    if (!backoffice) {
        backoffice = "/";
    }

    if (authService.isAuthenticated) {
        router.navigate([backoffice]);
        return false;
    }
    return true;
};
