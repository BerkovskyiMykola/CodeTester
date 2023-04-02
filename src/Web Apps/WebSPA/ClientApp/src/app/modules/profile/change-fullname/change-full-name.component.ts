import {Component} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ProfileService} from "../profile.service";
import {FullNameUpdateService} from "../full-name-update.service";

@Component({
  selector: 'app-change-full-name',
  templateUrl: './change-full-name.component.html',
  styleUrls: ['./change-full-name.component.css']
})
export class ChangeFullNameComponent {
  constructor(private profileService: ProfileService,
              public fullNameUpdateService: FullNameUpdateService) {
  }

  form = new FormGroup({
    fullName: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ])
  })
  get fullName() { return this.form.controls.fullName as FormControl }

  isFailed = false;
  isSuccessful = false;
  errorMessage = ''
  submit(){
    let model = {
      fullname: this.form.value.fullName as string
    }
    this.profileService.setProfileFullName(model).subscribe({
      next: (data) => {
        console.log(data);
        this.isFailed = false;
        this.fullNameUpdateService.changeName(model.fullname);
        this.isSuccessful = true;
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = "Something went wrong! Please, try later...";
        this.isFailed = true;
      }
    });
  }
}
