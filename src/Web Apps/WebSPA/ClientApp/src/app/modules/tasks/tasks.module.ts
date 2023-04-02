import { NgModule } from '@angular/core';
import {CommonModule, DatePipe} from '@angular/common';
import { TasksPageComponent } from './tasks-page/tasks-page.component';
import { TaskCardComponent } from './tasks-page/task-card/task-card.component';
import {ReactiveFormsModule} from "@angular/forms";
import { SolutionPageComponent } from './solution-page/solution-page.component';
import {RouterLink} from "@angular/router";
import { AttemptsPageComponent } from './attempts-page/attempts-page.component';
import { AllSolutionsPageComponent } from './all-solutions-page/all-solutions-page.component';
import {AceModule} from "ngx-ace-wrapper";



@NgModule({
  declarations: [
    TasksPageComponent,
    TaskCardComponent,
    SolutionPageComponent,
    AttemptsPageComponent,
    AllSolutionsPageComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    AceModule
  ],
  providers: [DatePipe]
})
export class TasksModule { }
