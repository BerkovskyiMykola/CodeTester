import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutoLoginPartialRoutesGuard } from 'angular-auth-oidc-client';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import {RegisterComponent} from "./modules/account/register/register.component";
import {ConfirmEmailComponent} from "./modules/account/confirm-email/confirm-email.component";
import {ResendConfirmEmailComponent} from "./modules/account/resend-confirm-email/resend-confirm-email.component";
import {ForgotPasswordComponent} from "./modules/account/forgot-password/forgot-password.component";
import {ResetPasswordComponent} from "./modules/account/reset-password/reset-password.component";
import {ProfileComponent} from "./modules/profile/profile.component";
import {TasksPageComponent} from "./modules/tasks/tasks-page/tasks-page.component";
import {SolutionPageComponent} from "./modules/tasks/solution-page/solution-page.component";
import {AttemptsPageComponent} from "./modules/tasks/attempts-page/attempts-page.component";
import {AllSolutionsPageComponent} from "./modules/tasks/all-solutions-page/all-solutions-page.component";
import {LoginComponent} from "./modules/login/login.component";

const routes: Routes = [
  // main
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  { path: 'home', component: HomeComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'login', component: LoginComponent, canActivate: [AutoLoginPartialRoutesGuard] },
  // account
  { path: 'account/register', component: RegisterComponent },
  { path: 'account/confirm-email', component: ConfirmEmailComponent },
  { path: 'account/resend-confirm-email', component: ResendConfirmEmailComponent },
  { path: 'account/forgot-password', component: ForgotPasswordComponent },
  { path: 'account/reset-password', component: ResetPasswordComponent },
  // profile
  { path: 'profile', component: ProfileComponent, canActivate: [AutoLoginPartialRoutesGuard] },
  // tasks
  { path: 'tasks', component: TasksPageComponent, canActivate: [AutoLoginPartialRoutesGuard] },
  { path: 'tasks/:id', component: SolutionPageComponent, canActivate: [AutoLoginPartialRoutesGuard] },
  { path: 'tasks/:id/attempts', component: AttemptsPageComponent, canActivate: [AutoLoginPartialRoutesGuard] },
  { path: 'tasks/:id/all-solutions', component: AllSolutionsPageComponent, canActivate: [AutoLoginPartialRoutesGuard] },

  // 404
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
