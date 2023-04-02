import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {IFullName} from "../shared/models/profile/full-name.model";
import {IUpdatePassword} from "../shared/models/profile/update-password.model";

const API = 'http://localhost:8010/api/v1/um/profile/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  responseType: 'text' as const
};

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private http: HttpClient) { }

  getProfileFullName(): Observable<IFullName> {
    return this.http.get<IFullName>(`${API}full-name`);
  }

  setProfileFullName(model: IFullName): Observable<any>{
    return this.http.put(`${API}full-name`, model, httpOptions);
  }

  updatePassword(model: IUpdatePassword): Observable<any>{
    return this.http.put(`${API}password`, model, httpOptions);
  }

  updatePhoto(file: File): Observable<any>{
    console.log(file)

    const formData = new FormData();
    formData.append('file', file);
    let headers = new HttpHeaders();
    headers = headers.append('enctype', 'multipart/form-data');
    return this.http.put(`${API}photo`, formData, { headers: headers })
  }

  deletePhoto(): Observable<any>{
    return this.http.delete(`${API}photo`, httpOptions);
  }
}
