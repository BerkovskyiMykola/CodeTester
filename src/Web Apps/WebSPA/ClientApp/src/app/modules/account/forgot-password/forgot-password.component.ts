import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ForgotPasswordService} from "./forgot-password.service";

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  constructor(private forgotPasswordService: ForgotPasswordService) {
  }

  form = new FormGroup({
    email: new FormControl<string>('',[
      Validators.required,
      Validators.email
    ])
  })
  isSuccessful = false;
  isFailed = false;
  errorMessage = ""


  get email() { return this.form.controls.email as FormControl }

  submit(){
    this.forgotPasswordService
      .forgotPassword(this.form.value.email as string)
      .subscribe({
        next: (data) => {
          console.log(data);
          this.isSuccessful = true;
          this.isFailed = false;
        },
        error: (err) => {
          console.log(err);
          this.errorMessage = "Something went wrong! Please, try again later...";
          this.isFailed = true;
        }
      });
  }
}
