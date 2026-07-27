import { AuthService, ConfigStateService, findRoute, getRoutePath, HttpErrorReporterService, RoutesService } from "@abp/ng.core";
import { inject } from "@angular/core";
import {
  Router,
  type ActivatedRouteSnapshot,
  type CanActivate,
  type CanActivateChildFn,
  type CanActivateFn,
  type RouterStateSnapshot,
  type UrlTree,
} from "@angular/router";
import { Observable, delay, of, tap } from "rxjs";
import { RoleService } from "../services";
import { HttpErrorResponse } from "@angular/common/http";

export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
): Observable<boolean> | boolean | UrlTree => {
  let roleService = inject(RoleService);
  let authService = inject(AuthService);
  let httpErrorReporter = inject(HttpErrorReporterService);
  let router = inject(Router);
  let routerService = inject(RoutesService);
  let { requiredRole } = route.data || {};

  // if (!requiredRole) {
  //   const routeFound = findRoute(routerService, getRoutePath(router, state.url));
  //   requiredRole = routeFound?.requiredRole;
  // }

  // si no se especifica un rol, se permite acceder
  if (!requiredRole) {
    return of(true);
  }

  let tempResult = roleService.getGrantedRole$(requiredRole).pipe(
    tap(access => {
      if (!access && authService.isAuthenticated) {
        httpErrorReporter.reportError({ status: 403 } as HttpErrorResponse);
      }
    })
  );
  return tempResult;
};