import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";

const API = 'http://localhost:8010/api/v1/um/account/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as const
};
@Injectable({
  providedIn: 'root'
})
export class ResendConfirmEmailService {
  constructor(private http: HttpClient) { }

  resendConfirmEmail(email: string): Observable<any>{
    let model = { "email": email }
    console.log(email)
    return this.http.post(`${API}resend-confirm-email`, model, httpOptions);
  }
}
