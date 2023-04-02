import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css'],
})
export class NavigationComponent implements OnInit {
  isAuthenticated: boolean = false;
  isCollapsed = false;
  token: string = "";
  constructor(public oidcSecurityService: OidcSecurityService
  ) {}

  ngOnInit() {
    this.oidcSecurityService.isAuthenticated$.subscribe(({ isAuthenticated }) => {
      this.isAuthenticated = isAuthenticated;

      console.log('authenticated: ', isAuthenticated);
    });
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  refreshSession() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoff().subscribe((result) => console.log(result));
  }
}
