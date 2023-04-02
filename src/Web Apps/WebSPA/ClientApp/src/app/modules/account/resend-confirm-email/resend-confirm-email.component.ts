import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ResendConfirmEmailService} from "./resend-confirm-email.service";

@Component({
  selector: 'app-resend-confirm-email',
  templateUrl: './resend-confirm-email.component.html',
  styleUrls: ['./resend-confirm-email.component.css']
})
export class ResendConfirmEmailComponent {
  constructor(private resendConfirmEmailService: ResendConfirmEmailService) {
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
    this.resendConfirmEmailService
      .resendConfirmEmail(this.form.value.email as string)
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
