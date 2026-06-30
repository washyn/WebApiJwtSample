import type { LoginInput, LoginOutput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  postByModel = (model: LoginInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LoginOutput>({
      method: 'POST',
      url: '/api/app/account/login',
      body: model,
    },
    { apiName: this.apiName,...config });
}