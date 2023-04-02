import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ForgotPasswordComponent} from "./forgot-password.component";
import {ReactiveFormsModule} from "@angular/forms";



@NgModule({
  declarations: [
    ForgotPasswordComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule
  ]
})
export class ForgotPasswordModule { }
