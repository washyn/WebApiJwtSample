import { AbpAuthResponse, ConfigStateService, IAuthService, LoginParams } from '@abp/ng.core';
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Params, Router } from '@angular/router';
import { Observable, delay, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CustomAuthService implements IAuthService {
  constructor(public confState: ConfigStateService, public router: Router) { }
  oidc: boolean = false;
  
  get isInternalAuth(): boolean {
    throw new Error('Method not implemented.');
  }
  get isAuthenticated(): boolean {
    return this.confState.getAll().currentUser.isAuthenticated;
  }
  init(): Promise<any> {
    return Promise.resolve();
  }
  logout(queryParams?: Params): Observable<any> {
    return new Observable();
  }
  navigateToLogin(queryParams?: Params): void {
    this.router.navigate(['/auth']);
  }
  login(params: LoginParams): Observable<any> {
    return new Observable();
  }
  loginUsingGrant(grantType: string, parameters: object, headers?: HttpHeaders): Promise<AbpAuthResponse> {
    throw new Error('Method not implemented.');
  }
  getAccessTokenExpiration(): number {
    throw new Error('Method not implemented.');
  }
  getRefreshToken(): string {
    throw new Error('Method not implemented.');
  }
  getAccessToken(): string {
    throw new Error('Method not implemented.');
  }
  refreshToken(): Promise<AbpAuthResponse> {
    throw new Error('Method not implemented.');
  }
}
