import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:5000';

export const environment = {
    production: true,
    application: {
        baseUrl,
        name: 'BookStore',
        logoUrl: '',
    },
    oAuthConfig: {
        issuer: '/',
        redirectUri: baseUrl,
        clientId: 'BookStore_App',
        responseType: 'code',
        scope: 'offline_access BookStore',
        requireHttps: true,
    },
    apis: {
        default: {
            url: '',
            rootNamespace: 'Acme.BookStore',
        },
    },
} as Environment;
