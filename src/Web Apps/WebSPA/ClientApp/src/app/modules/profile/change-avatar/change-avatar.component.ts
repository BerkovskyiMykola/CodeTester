import {Component, Input} from '@angular/core';
import {ProfileService} from "../profile.service";

@Component({
  selector: 'app-change-avatar',
  templateUrl: './change-avatar.component.html',
  styleUrls: ['./change-avatar.component.css']
})
export class ChangeAvatarComponent {
  constructor(private profileService: ProfileService) {
  }

  validateRequired(){
    this.isRequiredValidationError = !this.file;
  }

  validateFileExtension() {
    const extension = this.file!.name.split('.').pop();
    this.isExtensionValidationError = extension === undefined
      || !this.allowedExtensions.includes(extension);
  }

  validateFileSize() {
    this.isSizeValidationError = this.file!.size > this.allowedMaxSize;
  }

  validateFile(){
    this.validateRequired();
    if (!this.isRequiredValidationError){
      this.validateFileExtension();
      this.validateFileSize()
    }
    return !(this.isSizeValidationError
      || this.isExtensionValidationError
      || this.isRequiredValidationError)
  }

  file: File | null | undefined;
  isExtensionValidationError = false;
  isSizeValidationError = false;
  isRequiredValidationError = false;
  isFileValid = false;
  allowedExtensions = ['jpg', 'jpeg', 'png']
  allowedMaxSize = 2*1024*1024;

  onFileSelected(event: any) {
    this.file = event.target.files[0];
    this.isFileValid = this.validateFile()
    console.log(this.file)
  }

  isFailed = false;
  isSuccessful = false;
  errorMessage = ''

  @Input() refreshCall!: Function;

  submit(event: Event){
    event.preventDefault()
    if (!this.isFileValid) return;
    this.profileService.updatePhoto(this.file!).subscribe({
      next: (data) => {
        console.log(data);
        this.isFailed = false;
        this.isSuccessful = true;
        this.refreshCall();
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = "Something went wrong! Please, try later...";
        this.isFailed = true;
      }
    });
  }
}
