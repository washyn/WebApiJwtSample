import { HttpHandler, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize } from 'rxjs/operators';
import {
  HttpWaitService,
  IApiInterceptor,
  IS_EXTERNAL_REQUEST,
  SessionStateService,
} from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class OAuthApiInterceptor implements IApiInterceptor {
  constructor(
    private sessionState: SessionStateService,
    private httpWaitService: HttpWaitService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    this.httpWaitService.addRequest(request);
    const isExternalRequest = request.context?.get(IS_EXTERNAL_REQUEST);
    const newRequest = isExternalRequest
      ? request
      : request.clone({
          setHeaders: this.getAdditionalHeaders(request.headers),
        });

    return next
      .handle(newRequest)
      .pipe(finalize(() => this.httpWaitService.deleteRequest(request)));
  }

  getAdditionalHeaders(existingHeaders?: HttpHeaders) {
    const headers = {} as any;
    console.log(this.sessionState.getTenant());
    console.log('call to getAdditionalHeaders from api.interceptor');

    const lang = this.sessionState.getLanguage();
    if (!existingHeaders?.has('Accept-Language') && lang) {
      headers['Accept-Language'] = lang;
    }

    headers['X-Requested-With'] = 'XMLHttpRequest';

    return headers;
  }
}
