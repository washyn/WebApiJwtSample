// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { Environment } from '@abp/ng.core';

const baseUrl = 'https://localhost:4200';
// Improvement: app logo url
export const environment = {
    production: false,
    application: {
        baseUrl,
        name: 'BookStore',
        logoUrl: '',
    },
    oAuthConfig: {
        issuer: 'https://localhost:44357/',
        redirectUri: baseUrl,
        clientId: 'BookStore_App',
        responseType: 'code',
        scope: 'offline_access BookStore',
        requireHttps: true,
    },
    apis: {
        default: {
            url: 'https://localhost:44357',
            rootNamespace: '',
        },
    },
} as Environment;

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
