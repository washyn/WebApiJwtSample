import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpHeaders } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { HttpWaitService, IS_EXTERNAL_REQUEST, SessionStateService } from '@abp/ng.core';

export const oAuthApiInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const httpWaitService = inject(HttpWaitService);
  const sessionState = inject(SessionStateService);

  httpWaitService.addRequest(request);
  const isExternalRequest = request.context?.get(IS_EXTERNAL_REQUEST);

  const getAdditionalHeaders = (existingHeaders?: HttpHeaders) => {
    const headers = {} as any;
    console.log(sessionState.getTenant());
    console.log('call to getAdditionalHeaders from functional api interceptor');

    const lang = sessionState.getLanguage();
    if (!existingHeaders?.has('Accept-Language') && lang) {
      headers['Accept-Language'] = lang;
    }

    headers['X-Requested-With'] = 'XMLHttpRequest';

    return headers;
  };

  const newRequest = isExternalRequest
    ? request
    : request.clone({
        setHeaders: getAdditionalHeaders(request.headers),
      });

  return next(newRequest).pipe(
    finalize(() => httpWaitService.deleteRequest(request))
  );
};
