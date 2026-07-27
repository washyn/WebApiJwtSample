import { AuthService, ConfigStateService } from "@abp/ng.core";
import { inject } from "@angular/core";
import type {
  ActivatedRouteSnapshot,
  CanActivateChildFn,
  RouterStateSnapshot,
  UrlTree,
} from "@angular/router";
import { Observable, delay, of, tap } from "rxjs";
import { RoleService } from "../services";

/**
 * @deprecated use role.guard instead
 * @param childRoute 
 * @param state 
 * @returns 
 */
export const cashierRoleGuard: CanActivateChildFn = (
  childRoute: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
): Observable<boolean> | boolean | UrlTree => {
  const role = "cashier";
  let roleService = inject(RoleService);
  let isGrantedRole = roleService.getGrantedRole(role);
  return isGrantedRole;
};
