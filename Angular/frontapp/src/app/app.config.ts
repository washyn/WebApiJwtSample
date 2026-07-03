import {
  ApplicationConfig,
  ErrorHandler,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthService, provideAbpCore, withOptions } from '@abp/ng.core';
import { environment } from '../environments/environment';
import { registerLocaleForEsBuild } from '@abp/ng.core/locale';
import { CustomAuthService, OAuthApiInterceptor, provideAbpUtils } from './core';
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
import { provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { DEFAULT_HANDLERS_PROVIDERS } from './providers';
import { ErrorHandler as CustomErrorHandler } from './handlers/error.handler';
import { HTTP_ERROR_CONFIG } from './tokens';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useExisting: OAuthApiInterceptor,
      multi: true,
    },
    provideAbpUtils(),
    provideAbpCore(
      withOptions({
        environment,
        registerLocaleFn: registerLocaleForEsBuild(),
        // shoud be add this locale in back "AppName": "Pagos UNAJ" in back
        // localizations: [
        //   {
        //     culture: 'es',
        //     resources: [
        //       {
        //         resourceName: 'HelpDesk',
        //         texts: {
        //           AppName: 'Configurado en angular',
        //         },
        //       },
        //     ],
        //   },
        // ],
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
      // componente objetivo selector de clase
      provide: VALIDATION_TARGET_SELECTOR,
      useValue: '.field',
    },
    {
      // clase a agregar incencesario ya que el compontnet ya le agrega, pero se requiere el injector para q compile
      provide: VALIDATION_INVALID_CLASSES,
      useValue: 'field-error-class',
    },
    {
      provide: VALIDATION_BLUEPRINTS,
      // useValue: { ...BLUEPRINTS }, // default
      // para usar con abp y junto con el template para que funcione bien
      useValue: { ...DEFAULT_VALIDATION_BLUEPRINTS },
    },
    {
      provide: VALIDATION_ERROR_TEMPLATE,
      // useValue: ValidationErrorComponent, // default emplate
      useValue: CustomValidationErrorComponent,
    },
    { provide: HTTP_ERROR_CONFIG, useValue: undefined },
    {
      provide: ErrorHandler,
      useClass: CustomErrorHandler,
    },
    DEFAULT_HANDLERS_PROVIDERS,
  ],
};
// available tokens
// export * from './blueprints.token'; // USED
// export * from './error-template.token';// USED
// export * from './invalid-classes.token'; // USED
// export * from './map-errors-fn.token'; // USED
// export * from './target-selector.token'; // USED
// export * from './validate-on-submit.token'; // USED
