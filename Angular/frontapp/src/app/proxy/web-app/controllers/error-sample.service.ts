import type { ModelSample } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ErrorSampleService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  error400 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error400',
    },
    { apiName: this.apiName,...config });
  

  error401 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error401',
    },
    { apiName: this.apiName,...config });
  

  error403 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error403',
    },
    { apiName: this.apiName,...config });
  

  error404 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error404',
    },
    { apiName: this.apiName,...config });
  

  error40XXXByModelSample = (modelSample: ModelSample, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error40XXX',
      params: { testValue: modelSample.testValue, secondValue: modelSample.secondValue },
    },
    { apiName: this.apiName,...config });
  

  error500 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error500',
    },
    { apiName: this.apiName,...config });
  

  error501 = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error501',
    },
    { apiName: this.apiName,...config });
  

  errorBusinessException = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/error-business-exception',
    },
    { apiName: this.apiName,...config });
  

  largeRequest = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, object>({
      method: 'GET',
      url: '/api/app/error-sample/large-request',
    },
    { apiName: this.apiName,...config });
  

  largeRequestSecondExample = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, object>({
      method: 'GET',
      url: '/api/app/error-sample/large-request-second-example',
    },
    { apiName: this.apiName,...config });
  

  requireAuth = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: '/api/app/error-sample/require-auth',
    },
    { apiName: this.apiName,...config });
}