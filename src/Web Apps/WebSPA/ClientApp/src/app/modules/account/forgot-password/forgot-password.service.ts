import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {IResetPassword} from "../../shared/models/profile/reset-password.model";

const API = 'http://localhost:8010/api/v1/um/account/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as const
};
@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordService {
  constructor(private http: HttpClient) { }

  forgotPassword(email: string): Observable<any>{
    let model = { "email": email }
    console.log(email)
    return this.http.post(`${API}forgot-password`, model, httpOptions);
  }

  resetPassword(model: IResetPassword): Observable<any>{
    console.log(model)
    return this.http.post(`${API}reset-password`, model, httpOptions);
  }
}
