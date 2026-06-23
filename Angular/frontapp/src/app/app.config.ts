import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthService, provideAbpCore, withOptions } from '@abp/ng.core';
import { environment } from '../environments/environment';
import { registerLocaleForEsBuild } from '@abp/ng.core/locale';
import { CustomAuthService, oAuthApiInterceptor, provideAbpUtils } from './core';
import {
  defaultMapErrorsFn,
  VALIDATION_BLUEPRINTS,
  VALIDATION_ERROR_TEMPLATE,
  VALIDATION_INVALID_CLASSES,
  VALIDATION_MAP_ERRORS_FN,
  VALIDATION_TARGET_SELECTOR,
  VALIDATION_VALIDATE_ON_SUBMIT,
} from '@ngx-validate/core';
import { DEFAULT_VALIDATION_BLUEPRINTS, CustomValidationErrorComponent } from './shared';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([oAuthApiInterceptor])),
    provideAbpUtils(),
    provideAbpCore(
      withOptions({
        environment,
        registerLocaleFn: registerLocaleForEsBuild(),
      })
    ),
    // added for default missed auth service
    {
      provide: AuthService,
      useClass: CustomAuthService,
    },
    // "@ngx-validate/core" configuration
    {
      provide: VALIDATION_VALIDATE_ON_SUBMIT,
      useValue: true,
    },
    {
      provide: VALIDATION_MAP_ERRORS_FN,
      useValue: defaultMapErrorsFn,
    },
    {
      provide: VALIDATION_TARGET_SELECTOR,
      useValue: '.field',
    },
    {
      provide: VALIDATION_INVALID_CLASSES,
      useValue: 'field-error-class',
    },
    {
      provide: VALIDATION_BLUEPRINTS,
      useValue: { ...DEFAULT_VALIDATION_BLUEPRINTS },
    },
    {
      provide: VALIDATION_ERROR_TEMPLATE,
      useValue: CustomValidationErrorComponent,
    },
  ],
};

