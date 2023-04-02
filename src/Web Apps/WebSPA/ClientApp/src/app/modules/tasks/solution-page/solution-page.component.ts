import {Component, OnInit} from '@angular/core';
import {TasksService} from "../tasks.service";
import {ActivatedRoute} from "@angular/router";
import {ITaskDetails} from "../../shared/models/tasks/task-details.model";
import {ISolutionCheckResponse} from "../../shared/models/solutions/solution-check-response.model";

@Component({
  selector: 'app-solution-page',
  templateUrl: './solution-page.component.html',
  styleUrls: ['./solution-page.component.css']
})
export class SolutionPageComponent implements OnInit {
  constructor(
    private tasksService: TasksService,
    private route: ActivatedRoute
  ) {
  }

  solutionFormValue = "<your code goes here>"

  taskDetails!: ITaskDetails;

  isNotesLineShown = false;
  isTaskError = false;
  taskErrorMessage = "";
  isTaskLoading = false;
  isSolved = false;

  isSolutionFail = false;
  solutionFailMessage = "";
  isSolutionSuccess = false;
  isEditorSuccess = false;
  isSolutionSending = false;
  isSolutionSendingError = false;
  solutionSendingErrorMessage = ""

  solutionCheckResponse: ISolutionCheckResponse | undefined

  ngOnInit(): void {
    this.isTaskLoading = true;
    this.route.params
      .subscribe(params => {
        this.fetchDetails(params.id);
      })
  }

  updateCheckInfo(){
    if (this.solutionCheckResponse?.success){
      this.isSolutionSuccess = true;
      this.isEditorSuccess = true;
      this.isSolved = true;
    } else {
      this.isSolutionFail = true;
      this.solutionFailMessage = this.solutionCheckResponse?.message ?? "Incorrect solution"
    }
  }

  sendSolution(){
    this.isSolutionSending = true;
    this.isSolutionFail = false;
    this.isSolutionSendingError = false;
    this.isSolutionSuccess = false;
    this.isEditorSuccess = false;

    this.tasksService.sendSolution(this.taskDetails.id, this.solutionFormValue).subscribe({
      next: (data) => {
        console.log(data);
        this.isSolutionSending = false;
        this.solutionCheckResponse = data;
        this.updateCheckInfo();
      },
      error: (err) => {
        console.log(err);
        this.isSolutionSending = false;
        this.isSolutionSendingError = true;
        this.solutionSendingErrorMessage = 'Something went wrong. Please, try later!'
      }
    })
  }


  fetchDetails(taskId: string){
    this.tasksService.getTask(taskId).subscribe({
      next: (data) => {
        console.log(data);
        this.taskDetails = data;

        this.solutionFormValue = data.solutionTemplate;
        this.tryFetchLastSuccessSolutionAttempt()
        this.isTaskLoading = false;
      },
      error: (err) => {
        console.log(err);
        this.isTaskError = true;
        this.isTaskLoading = false;
        this.taskErrorMessage = "Task was not found!";
      }
    })
  }

  tryFetchLastSuccessSolutionAttempt(){
    this.tasksService.getLastSuccessSolutionAttempt(this.taskDetails.id).subscribe({
      next: (data) => {
        console.log(data)
        this.isSolved = true;
        this.isEditorSuccess = true;
        this.solutionFormValue = data.code;
      },
      error: (err) => {
        console.log(err)
        this.isSolved = false;
      }
    })
  }
}
