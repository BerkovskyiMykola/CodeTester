import {Component} from '@angular/core';
import {ProfileService} from "../profile.service";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  constructor(private profileService: ProfileService) {
  }

  form = new FormGroup({
    currentPassword: new FormControl<string>('', [
      Validators.required
    ]),
    newPassword: new FormControl<string>('', [
      Validators.required,
      Validators.pattern(`^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$`)
    ]),
    confirmNewPassword: new FormControl<string>('')
  }, [
    this.matchPasswordValidator
  ])
  matchPasswordValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmNewPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  get currentPassword() { return this.form.controls.currentPassword as FormControl }
  get newPassword() { return this.form.controls.newPassword as FormControl }
  get confirmNewPassword() { return this.form.controls.confirmNewPassword as FormControl }

  isFailed = false;
  isSuccessful = false;
  errorMessage = ''
  submit(){
    this.profileService.updatePassword({
      currentPassword: this.form.value.currentPassword as string,
      newPassword: this.form.value.newPassword as string
    }).subscribe({
      next: (data) => {
        console.log(data);
        this.isFailed = false;
        this.isSuccessful = true;
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = "Old password is wrong! Please, check it and try again...";
        this.isFailed = true;
      }
    });
  }
}
