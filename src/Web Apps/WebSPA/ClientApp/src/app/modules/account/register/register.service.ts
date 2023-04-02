import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {IRegister} from "../../shared/models/profile/register.model";
import {Observable} from "rxjs";

const AUTH_API = 'http://localhost:8010/api/v1/um/account/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as const
};

@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  constructor(private http: HttpClient) { }

  register(model: IRegister): Observable<any> {
    return this.http.post(`${AUTH_API}register`, model, httpOptions);
  }

  confirmEmail(userId: string, token: string): Observable<any>{
    let model = { "userId": userId, "token": token }
    console.log(model)
    return this.http.post(`${AUTH_API}confirm-email`, model, httpOptions);
  }
}
