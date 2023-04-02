import {Component, OnInit} from '@angular/core';
import {ProfileService} from "./profile.service";
import {FullNameUpdateService} from "./full-name-update.service";
import {OidcSecurityService} from "angular-auth-oidc-client";
import {BehaviorSubject} from "rxjs";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit{
  constructor(private profileService: ProfileService,
              public fullNameUpdateService: FullNameUpdateService,
              private oidcSecurityService: OidcSecurityService
  ) {
  }

  isModalForPasswordOpen$ = new BehaviorSubject(false);
  isModalForFullNameOpen$ = new BehaviorSubject(false);
  isModalForAvatarOpen$ = new BehaviorSubject(false);

  baseAvatarUrl = "http://localhost:8010/api/v1/um/pictures/"
  userAvatarUrl = "";
  defaultPicture = "assets/images/user-icon.png"
  userId!: string;
  isDeleteError = false;
  errorMessage = '';

  ngOnInit(): void {
    this.refreshFullName();

    this.oidcSecurityService.userData$.subscribe(userData=>{
      this.userId = userData.userData.sub;
    })

    console.log(this.userId)
    this.userAvatarUrl = `${this.baseAvatarUrl}${this.userId}/photo`
  }

  refreshFullName(){
    console.log("refreshing full name...")
    this.profileService.getProfileFullName().subscribe(fullNameModel => {
      console.log(fullNameModel)
      this.fullNameUpdateService.changeName(fullNameModel.fullname)
    })
  }

  refreshUserAvatar(){
    console.log("Refreshing avatar...")
    this.userAvatarUrl = `${this.baseAvatarUrl}${this.userId}/photo?${new Date().getTime()}`;
  }

  deleteAvatar(){
    let confirmation = confirm("Are you sure you want to delete your avatar?")
    console.log(confirmation)
    if (!confirmation) return;
    this.profileService.deletePhoto().subscribe({
      next: (data) => {
        console.log(data);
        this.changeAvatarToDefault();
      },
      error: (err) => {
        console.log(err);
        this.isDeleteError = true;
        this.errorMessage = "Something went wrong... Please, try later!";
      }
    })
  }

  changeAvatarToDefault() {
    this.userAvatarUrl = this.defaultPicture;
  }
}
