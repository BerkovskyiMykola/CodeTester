import {Component, Input} from '@angular/core';
import {BehaviorSubject} from "rxjs";

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent {
  @Input() title = ""
  @Input() isOpen = new BehaviorSubject(false);
}
