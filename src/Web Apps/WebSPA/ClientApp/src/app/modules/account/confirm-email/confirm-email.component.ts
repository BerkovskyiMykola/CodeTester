import {Component, OnInit} from '@angular/core';
import {RegisterService} from "../register/register.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit{
  constructor(
    private registerService: RegisterService,
    private route: ActivatedRoute
  ) {
  }
  isSuccessful: boolean = false;
  isFailed: boolean = false;
  isNotValidLink: boolean = false;

  ngOnInit(): void {
    this.route.queryParams
      .subscribe(params => {
        console.log(`userId: ${params.userId}, token: ${encodeURI(params.token)}`)
        if (!('userId' in params && 'token' in params)){
          this.isNotValidLink = true;
          return;
        }

        this.registerService.confirmEmail(params.userId, params.token).subscribe({
          next: (data) => {
            console.log(data);
            this.isSuccessful = true;
          },
          error: (err) => {
            console.log(err);
            this.isFailed = true;
          }
        });
      })
  }

}
