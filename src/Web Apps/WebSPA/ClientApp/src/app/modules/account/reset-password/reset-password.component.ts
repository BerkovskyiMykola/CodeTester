import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import {ForgotPasswordService} from "../forgot-password/forgot-password.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit{

  constructor(private forgotPasswordService: ForgotPasswordService,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.queryParams
      .subscribe(params => {
        console.log(`email: ${params.email}, token: ${encodeURI(params.token)}`)
        if (!('email' in params && 'token' in params)){
          this.isNotValidLink = true;
          return;
        }
        this.email = params.email;
        this.token = encodeURI(params.token);
      })
  }

  form = new FormGroup({
    password: new FormControl<string>('', [
      Validators.required,
      Validators.pattern(`^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$`)
    ]),
    confirmPassword: new FormControl<string>('')
  }, [
    this.matchPasswordValidator
  ])
  isSuccessful = false;
  isFailed = false;
  errorMessage = '';
  isNotValidLink = false;
  email = "";
  token = "";
  submitDone = false;

  matchPasswordValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }
  get password() { return this.form.controls.password as FormControl }
  get confirmPassword() { return this.form.controls.confirmPassword as FormControl }


  submit(){
    this.submitDone = true;
    this.forgotPasswordService.resetPassword({
      email: this.email,
      token: this.token,
      password: this.form.value.password as string
    }).subscribe({
      next: (data) => {
        console.log(data);
        this.isSuccessful = true;
        this.isFailed = false;
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = "Something went wrong! Check your verification link and try later...";
        this.isFailed = true;
        this.submitDone = false;
      }
    });
  }
}
