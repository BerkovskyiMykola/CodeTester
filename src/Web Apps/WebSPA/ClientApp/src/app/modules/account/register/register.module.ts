import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RegisterComponent} from "./register.component";
import {ReactiveFormsModule} from "@angular/forms";
import {RouterLink} from "@angular/router";



@NgModule({
  declarations: [
    RegisterComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ]
})
export class RegisterModule { }
