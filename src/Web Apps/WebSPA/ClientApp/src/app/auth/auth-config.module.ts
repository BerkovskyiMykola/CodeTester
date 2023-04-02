import { NgModule } from '@angular/core';
import { AuthModule, LogLevel } from 'angular-auth-oidc-client';

@NgModule({
  imports: [
    AuthModule.forRoot({
      config: {
        triggerAuthorizationResultEvent: true,
        postLoginRoute: '/home',
        forbiddenRoute: '/forbidden',
        unauthorizedRoute: '/unauthorized',
        secureRoutes: ['http://localhost:8010'],
        logLevel: LogLevel.Debug,
        historyCleanupOff: true,
        authority: 'http://localhost:8000',
        redirectUrl: window.location.origin,
        postLogoutRedirectUri: window.location.origin,
        clientId: 'angular_spa',
        scope: 'openid profile roles usermanagement dictionary testing',
        responseType: 'code'
      },
    }),
  ],
  exports: [AuthModule],
})
export class AuthConfigModule {}
