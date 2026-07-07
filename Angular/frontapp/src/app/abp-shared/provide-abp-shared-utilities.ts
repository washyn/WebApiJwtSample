import { HTTP_INTERCEPTORS } from '@angular/common/http';
import {
  APP_INITIALIZER,
  Provider,
  Type,
  makeEnvironmentProviders,
} from '@angular/core';
import {
  Validation,
  VALIDATION_BLUEPRINTS,
  VALIDATION_ERROR_TEMPLATE,
  VALIDATION_INVALID_CLASSES,
  VALIDATION_MAP_ERRORS_FN,
  VALIDATION_TARGET_SELECTOR,
  VALIDATION_VALIDATE_ON_SUBMIT,
  defaultMapErrorsFn,
} from '@ngx-validate/core';
import { DEFAULT_VALIDATION_BLUEPRINTS } from './constants';
import { ErrorHandler } from './handlers';
import { HttpErrorConfig } from './models';
import { DEFAULT_HANDLERS_PROVIDERS } from './providers';
import { OAuthApiInterceptor, provideAbpUtils } from './core';
import { CustomValidationErrorComponent } from './shared';
import { HTTP_ERROR_CONFIG } from './tokens';

export interface AbpSharedUtilitiesOptions {
  httpErrorConfig?: HttpErrorConfig;
  validationBluePrints?: Validation.Blueprints;
  validationMapErrorsFn?: Validation.MapErrorsFn;
  validateOnSubmit?: boolean;
  validationTargetSelector?: string;
  validationInvalidClasses?: string;
  validationErrorComponent?: Type<unknown>;
  registerHttpInterceptor?: boolean;
}

export function getAbpSharedUtilityProviders(
  options: AbpSharedUtilitiesOptions = {}
): Provider[] {
  const providers: Provider[] = [
    {
      provide: APP_INITIALIZER,
      multi: true,
      deps: [ErrorHandler],
      useFactory: () => () => undefined,
    },
    provideAbpUtils(),
    {
      provide: VALIDATION_VALIDATE_ON_SUBMIT,
      useValue: options.validateOnSubmit ?? true,
    },
    {
      provide: VALIDATION_MAP_ERRORS_FN,
      useValue: options.validationMapErrorsFn ?? defaultMapErrorsFn,
    },
    {
      provide: VALIDATION_TARGET_SELECTOR,
      useValue: options.validationTargetSelector ?? '.field',
    },
    {
      provide: VALIDATION_INVALID_CLASSES,
      useValue: options.validationInvalidClasses ?? 'field-error-class',
    },
    {
      provide: VALIDATION_BLUEPRINTS,
      useValue: {
        ...DEFAULT_VALIDATION_BLUEPRINTS,
        ...(options.validationBluePrints || {}),
      },
    },
    {
      provide: VALIDATION_ERROR_TEMPLATE,
      useValue: options.validationErrorComponent ?? CustomValidationErrorComponent,
    },
    {
      provide: HTTP_ERROR_CONFIG,
      useValue: options.httpErrorConfig,
    },
    DEFAULT_HANDLERS_PROVIDERS,
  ];

  if (options.registerHttpInterceptor !== false) {
    providers.unshift({
      provide: HTTP_INTERCEPTORS,
      useExisting: OAuthApiInterceptor,
      multi: true,
    });
  }

  return providers;
}

export function provideAbpSharedUtilities(
  options: AbpSharedUtilitiesOptions = {}
) {
  return makeEnvironmentProviders(getAbpSharedUtilityProviders(options));
}
