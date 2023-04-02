import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {IGetTasksRequest} from "../shared/models/tasks/get-tasks-request.model";
import {Observable} from "rxjs";
import {IPageView} from "../shared/models/general/page-view.model";
import {ITaskDetails} from "../shared/models/tasks/task-details.model";
import {ISolutionCheckResponse} from "../shared/models/solutions/solution-check-response.model";
import {IAttempt} from "../shared/models/solutions/attempt.model";
import {ISolutionsRequest} from "../shared/models/solutions/solutions-request.model";
import {ISolution} from "../shared/models/solutions/solution.model";

const API = 'http://localhost:8010/api/v1/t/CodeChallenge/tasks/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  ContentType: 'application/json'
};

@Injectable({
  providedIn: 'root'
})
export class TasksService {

  constructor(private http: HttpClient) { }

  getTasksList(model: IGetTasksRequest): Observable<IPageView<ITaskDetails>>{
    const params = model as { [param: string]: string | string[] };
    return this.http.get<IPageView<ITaskDetails>>(`${API}`,
      {params: params, headers: httpOptions.headers});
  }

  getTask(taskId: string): Observable<ITaskDetails>{
    return this.http.get<ITaskDetails>(`${API}${taskId}`, httpOptions);
  }

  getLastSuccessSolutionAttempt(taskId: string): Observable<IAttempt>{
    return this.http.get<IAttempt>(`${API}${taskId}/last-success-solution-attempt`, httpOptions)
  }

  sendSolution(taskId: string, solutionValue: string): Observable<ISolutionCheckResponse>{
    let body = {
      code: solutionValue
    }
    return this.http.post<ISolutionCheckResponse>(`${API}${taskId}/solution-attempt/run`, body, httpOptions);
  }

  getSolutionAttempts(model: ISolutionsRequest): Observable<IPageView<IAttempt>>{
    const { id, ...params } = model;
    return this.http.get<IPageView<IAttempt>>(`${API}${id}/solution-attempts`, {
      params,
      headers: httpOptions.headers
    });
  }

  getAllSolutions(model: ISolutionsRequest): Observable<IPageView<ISolution>>{
    const { id, ...params } = model;
    return this.http.get<IPageView<ISolution>>(`${API}${id}/solutions`, {
      params,
      headers: httpOptions.headers
    });
  }
}
