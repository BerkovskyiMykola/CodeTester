import { Component } from '@angular/core';
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import {RegisterService} from "./register.service";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  constructor(private registerService: RegisterService) {
  }

  form = new FormGroup({
    fullName: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    email: new FormControl<string>('',[
      Validators.required,
      Validators.email
    ]),
    password: new FormControl<string>('', [
      Validators.required,
      Validators.pattern(`^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$`)
    ]),
    confirmPassword: new FormControl<string>('')
  }, [
    this.matchPasswordValidator
  ])
  isSuccessful = false;
  isSignUpFailed = false;
  errorMessage = '';

  matchPasswordValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  get fullName() { return this.form.controls.fullName as FormControl }
  get email() { return this.form.controls.email as FormControl }
  get password() { return this.form.controls.password as FormControl }
  get confirmPassword() { return this.form.controls.confirmPassword as FormControl }


  submit(){
    this.registerService.register({
      fullName: this.form.value.fullName as string,
      email: this.form.value.email as string,
      password: this.form.value.password as string
    }).subscribe({
      next: (data) => {
        console.log(data);
        this.isSuccessful = true;
        this.isSignUpFailed = false;
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = "This email is already taken!";
        this.isSignUpFailed = true;
      }
    });
  }
}
