import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RangeService {
  range(start: number, end: number) {
    const length = end - start;
    return Array.from({ length }, (_, i) => start + i);
  }
}
