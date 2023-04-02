import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileComponent } from './profile.component';
import {ReactiveFormsModule} from "@angular/forms";
import {SharedModule} from "../shared/shared.module";
import { ChangeFullNameComponent } from './change-fullname/change-full-name.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ChangeAvatarComponent } from './change-avatar/change-avatar.component';



@NgModule({
  declarations: [
    ProfileComponent,
    ChangeFullNameComponent,
    ChangePasswordComponent,
    ChangeAvatarComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class ProfileModule { }
