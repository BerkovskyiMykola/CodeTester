import {Component, Input} from '@angular/core';
import {ITaskCard} from "../../../shared/models/tasks/task-card.model";

@Component({
  selector: 'app-task-card',
  templateUrl: './task-card.component.html',
  styleUrls: ['./task-card.component.css']
})
export class TaskCardComponent {
  @Input() taskInfo!: ITaskCard
}
