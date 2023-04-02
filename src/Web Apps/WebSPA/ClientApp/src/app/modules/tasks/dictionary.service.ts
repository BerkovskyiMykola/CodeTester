import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {IDifficulty} from "../shared/models/dictionary/difficulty.model";
import {ITaskType} from "../shared/models/dictionary/task-type.model";
import {IProgrammingLanguage} from "../shared/models/dictionary/programming-language";

const API = 'http://localhost:8010/api/v1/d/';

@Injectable({
  providedIn: 'root'
})
export class DictionaryService {

  constructor(private http: HttpClient) { }

  getDifficulties(): Observable<IDifficulty[]>{
    return this.http.get<IDifficulty[]>(`${API}difficulties`)
  }

  getTaskTypes(): Observable<ITaskType[]>{
    return this.http.get<ITaskType[]>(`${API}taskTypes`)
  }

  getProgrammingLanguages(): Observable<IProgrammingLanguage[]>{
    return this.http.get<IProgrammingLanguage[]>(`${API}programmingLanguages`)
  }
}
