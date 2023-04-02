import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ResendConfirmEmailComponent} from "./resend-confirm-email.component";
import { ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    ResendConfirmEmailComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule
  ]
})
export class ResendConfirmEmailModule { }
