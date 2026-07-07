import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthService, provideAbpCore, withOptions } from '@abp/ng.core';
import { environment } from '../environments/environment';
import { registerLocaleForEsBuild } from '@abp/ng.core/locale';
import { provideHttpClient } from '@angular/common/http';
import { provideAbpSharedUtilities } from './abp-shared';
import { CustomAuthService } from './core/services/custom-auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideAbpSharedUtilities(),
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
  ],
};
// available tokens
// export * from './blueprints.token'; // USED
// export * from './error-template.token';// USED
// export * from './invalid-classes.token'; // USED
// export * from './map-errors-fn.token'; // USED
// export * from './target-selector.token'; // USED
// export * from './validate-on-submit.token'; // USED
