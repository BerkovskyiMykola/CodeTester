import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import 'brace';
import 'brace/mode/text';
import 'brace/mode/javascript';
import 'brace/mode/csharp';
import 'brace/mode/python';

import 'brace/theme/crimson_editor.js';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './modules/shared/shared.module';
import { AuthConfigModule } from './auth/auth-config.module';
import { NavigationComponent } from './navigation/navigation.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HomeComponent } from './home/home.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from "@angular/common";
import { RegisterModule } from "./modules/account/register/register.module";
import { ReactiveFormsModule } from "@angular/forms";
import { ResendConfirmEmailModule } from "./modules/account/resend-confirm-email/resend-confirm-email.module";
import { ForgotPasswordModule } from "./modules/account/forgot-password/forgot-password.module";
import { ResetPasswordModule } from "./modules/account/reset-password/reset-password.module";
import { ConfirmEmailModule } from "./modules/account/confirm-email/confirm-email.module";
import { AuthInterceptor } from "angular-auth-oidc-client";
import { ProfileModule } from "./modules/profile/profile.module";
import { TasksModule } from "./modules/tasks/tasks.module";
import {AceModule} from "ngx-ace-wrapper";
import {LoginModule} from "./modules/login/login.module";

@NgModule({
  declarations: [
    AppComponent,
    NavigationComponent,
    UnauthorizedComponent,
    PageNotFoundComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AuthConfigModule,
    LoginModule,
    AppRoutingModule,
    SharedModule.forRoot(),
    AuthConfigModule,
    RegisterModule,
    ResendConfirmEmailModule,
    ConfirmEmailModule,
    ResetPasswordModule,
    ForgotPasswordModule,
    ProfileModule,
    TasksModule,

    AceModule,
    HttpClientModule,
    NgbModule,
    BrowserAnimationsModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
