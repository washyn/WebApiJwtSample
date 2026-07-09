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
import { ConsoleMessageService, ConsoleNotifyService, ConsoleUIService, IMessageService, INotifyService, IUIService, OAuthApiInterceptor, provideAbpUtils } from './core';
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
    ////////////////////////////////////////////////////////////////////////////
    // provideAbpUtils(),
    {
      provide: INotifyService,
      useClass: ConsoleNotifyService
    },
    {
      provide: IMessageService,
      useClass: ConsoleMessageService
    },
    {
      provide: IUIService,
      useClass: ConsoleUIService
    },
    ////////////////////////////////////////////////////////////////////////////
    {
      provide: VALIDATION_VALIDATE_ON_SUBMIT,
      useValue: options.validateOnSubmit ?? true,
    },
    {
      provide: VALIDATION_MAP_ERRORS_FN,
      useValue: options.validationMapErrorsFn ?? defaultMapErrorsFn,
    },
    {
      // componente objetivo selector de clase, parent wraper of field(label, input)
      provide: VALIDATION_TARGET_SELECTOR,
      useValue: options.validationTargetSelector ?? '.field',
    },
    {
      // clase a agregar incencesario ya que el compontnet ya le agrega, pero se requiere el injector para q compile
      provide: VALIDATION_INVALID_CLASSES,
      useValue: options.validationInvalidClasses ?? 'field-error-class',
    },
    {
      // useValue: { ...BLUEPRINTS }, // default
      // para usar con abp y junto con el template para que funcione bien
      provide: VALIDATION_BLUEPRINTS,
      useValue: {
        ...DEFAULT_VALIDATION_BLUEPRINTS,
        ...(options.validationBluePrints || {}),
      },
    },
    {
      // useValue: ValidationErrorComponent, // default emplate
      provide: VALIDATION_ERROR_TEMPLATE,
      useValue: options.validationErrorComponent ?? CustomValidationErrorComponent,
    },
    //////////////////////////////////////////////////////////////////////////
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
// available tokens
// export * from './blueprints.token'; // USED
// export * from './error-template.token';// USED
// export * from './invalid-classes.token'; // USED
// export * from './map-errors-fn.token'; // USED
// export * from './target-selector.token'; // USED
// export * from './validate-on-submit.token'; // USED
