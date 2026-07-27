import type { CanActivateFn, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

// IMPROVEMENT: add activate for child and validate role admin... add types


/**
 * @deprecated not implemented
 * @param route 
 * @param state 
 * @returns 
 */
export const cutomActivateGuard: CanActivateFn = (
    route,
    state
): Observable<boolean> | boolean | UrlTree => {
    return true;
};
