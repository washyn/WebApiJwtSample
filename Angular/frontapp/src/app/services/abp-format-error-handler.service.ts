import { inject, Injectable } from '@angular/core';
import { AuthService, LocalizationService } from '@abp/ng.core';
import { HttpErrorResponse } from '@angular/common/http';
import { getErrorFromRequestBody } from '../utils/error.utils';
import { CustomHttpErrorHandlerService } from '../models/common';
// import { ConfirmationService } from '../services/confirmation.service';
import { CUSTOM_HTTP_ERROR_HANDLER_PRIORITY } from '../constants/default-errors';
import { AbpUtilService } from '../core/abp-utils/abp-util.service';

@Injectable({ providedIn: 'root' })
export class AbpFormatErrorHandlerService implements CustomHttpErrorHandlerService {
  readonly priority = CUSTOM_HTTP_ERROR_HANDLER_PRIORITY.high;
  // private confirmationService = inject(ConfirmationService);
  private abpUtilService = inject(AbpUtilService);
  private localizationService = inject(LocalizationService);
  private authService = inject(AuthService);
  private error: HttpErrorResponse | undefined = undefined;

  private navigateToLogin() {
    return this.authService.navigateToLogin();
  }

  canHandle(error: unknown): boolean {
    if (error instanceof HttpErrorResponse && error.headers.get('_AbpErrorFormat')) {
      this.error = error;
      return true;
    }
    return false;
  }

  execute() {
    console.log('AbpFormatErrorHandlerService');
    const { message, title } = getErrorFromRequestBody(this.error?.error?.error);
    ///////////////////////////////////////////////////////
    // this.confirmationService
    //   .error(message, title, {
    //     hideCancelBtn: true,
    //     yesText: 'AbpAccount::Close',
    //   })
    //   .subscribe(() => {
    //     if (this.error?.status === 401) {
    //       this.navigateToLogin();
    //     }
    //   });
    ///////////////////////////////////////////////////////
    let messageLocalized = this.localizationService.instant(message);
    let titleLocalized = this.localizationService.instant(title);

    // TODO: as improvement can be return a observable with same result of original service, in default message service
    this.abpUtilService.message.error(messageLocalized, titleLocalized);

    setTimeout(() => {
      if (this.error?.status === 401) {
        this.navigateToLogin();
      }
    }, 2000);
  }
}
