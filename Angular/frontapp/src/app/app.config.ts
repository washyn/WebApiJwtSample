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
import { CustomAuthService } from './custom-auth.service';
import {
  BLUEPRINTS,
  defaultMapErrorsFn,
  VALIDATION_BLUEPRINTS,
  VALIDATION_ERROR_TEMPLATE,
  VALIDATION_INVALID_CLASSES,
  VALIDATION_MAP_ERRORS_FN,
  VALIDATION_TARGET_SELECTOR,
  VALIDATION_VALIDATE_ON_SUBMIT,
  ValidationErrorComponent,
} from '@ngx-validate/core';
import { DEFAULT_VALIDATION_BLUEPRINTS } from './validation';
import { CustomValidationErrorComponent } from './validation-error.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { OAuthApiInterceptor } from './api.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAbpCore(
      withOptions({
        environment,
        registerLocaleFn: registerLocaleForEsBuild(),
      })
    ),
    // aded for default missed auth service
    {
      provide: AuthService,
      useClass: CustomAuthService,
    },
    //"@ngx-validate/core": "^0.2.0", for validation
    {
      provide: VALIDATION_VALIDATE_ON_SUBMIT,
      useValue: true,
    },
    {
      provide: VALIDATION_MAP_ERRORS_FN,
      useValue: defaultMapErrorsFn,
    },
    {
      provide: VALIDATION_TARGET_SELECTOR, // componente objetivo selector de clase
      useValue: '.field',
    },
    {
      provide: VALIDATION_INVALID_CLASSES, // clase a agregar incencesario ya que el compontnet ya le agrega, pero se requiere el injector para q compile
      useValue: 'field-error-class',
    },
    {
      provide: VALIDATION_BLUEPRINTS,
      // useValue: { ...BLUEPRINTS }, // default
      useValue: { ...DEFAULT_VALIDATION_BLUEPRINTS }, // para usar con abp y junto con el template para que funcione bien
    },
    {
      provide: VALIDATION_ERROR_TEMPLATE,
      // useValue: ValidationErrorComponent, // default emplate
      useValue: CustomValidationErrorComponent,
    },
    // interceptors
    {
      provide: HTTP_INTERCEPTORS,
      useExisting: OAuthApiInterceptor,
      multi: true,
    },
  ],
};
// available tokens
// export * from './blueprints.token'; // USED
// export * from './error-template.token';// USED
// export * from './invalid-classes.token'; // USED
// export * from './map-errors-fn.token'; // USED
// export * from './target-selector.token'; // USED
// export * from './validate-on-submit.token'; // USED
