import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class FullNameUpdateService {
  fullName$ = new BehaviorSubject<string>("");
  changeName(newValue: string){
    this.fullName$.next(newValue)
  }
}
