import { Component } from '@angular/core';
import {TasksService} from "../tasks.service";
import {ActivatedRoute, NavigationExtras, Params, Router} from "@angular/router";
import {DatePipe} from "@angular/common";
import {RangeService} from "../../shared/services/range.service";
import {ITaskDetails} from "../../shared/models/tasks/task-details.model";
import {IPageView} from "../../shared/models/general/page-view.model";
import {FormControl, FormGroup} from "@angular/forms";
import {ISolutionsRequest} from "../../shared/models/solutions/solutions-request.model";
import {ISolution} from "../../shared/models/solutions/solution.model";

@Component({
  selector: 'app-all-solutions-page',
  templateUrl: './all-solutions-page.component.html',
  styleUrls: ['./all-solutions-page.component.css']
})
export class AllSolutionsPageComponent {
  constructor(
    private tasksService: TasksService,
    private route: ActivatedRoute,
    public datePipe: DatePipe,
    private rangeService: RangeService,

    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.route.queryParams
      .subscribe(params => {
        this.writeParamsToFormGroup(params)
      })

    this.route.params
      .subscribe(params => {
        this.taskId = params.id;
        this.fetchSolutions();
        this.fetchTaskDetails();
      })
  }

  writeParamsToFormGroup(params: Params){
    const pageNumber = params.pageNumber ? parseInt(params.pageNumber) : 1;
    const pageSize = params.pageSize ? parseInt(params.pageSize) : 10;

    const model = {
      pageNumber,
      pageSize
    }

    this.filterForm.patchValue(model);
  }

  isLoading = false;
  taskId = "";
  taskDetails: ITaskDetails | undefined;
  isError = false;
  errorMessage = "";
  isEmptyResult = false;


  solutionsViewModel: IPageView<ISolution> = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 10,
    totalCount: 0,
    hasPrevious: false,
    hasNext: false,
    rows: []
  }

  filterForm = new FormGroup({
    pageNumber: new FormControl<number>(1),
    pageSize: new FormControl<number>(10),
  })

  pageCountVariants = [1, 2, 5, 10, 25]

  getRequestModel(): ISolutionsRequest{
    const pageNumber = this.filterForm.value.pageNumber;
    const pageSize = this.filterForm.value.pageSize;

    return {
      id: this.taskId,
      ...(pageNumber && {pageNumber: pageNumber}),
      ...(pageSize && {pageSize: pageSize})
    };
  }

  checkForEmptyResult(){
    if (this.solutionsViewModel.rows.length === 0){
      this.isEmptyResult = true;
      return;
    }
    this.isEmptyResult = false;
  }

  fetchSolutions(){
    const request = this.getRequestModel()

    this.tasksService.getAllSolutions(request).subscribe({
      next: (data) => {
        console.log(data);
        this.solutionsViewModel = data;
        this.isLoading = false;
        this.checkForEmptyResult()
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.isLoading = false;
        this.handleError(err)
      }
    })

    //change queries in link
    this.updateRouteWithParams(request)
  }

  handleError(err: any){
    if (err.status === 403){
      this.errorMessage = "You haven`t solved this task yet!";
    }
    else {
      this.errorMessage = "Something went wrong while fetching solutions from server... Please, try later!"
    }
  }

  updateRouteWithParams(requestModel: ISolutionsRequest){
    const queryParams: { [key: string]: any } = {};
    for (const [key, value] of Object.entries(requestModel)) {
      if (key != 'id')
        queryParams[key] = value;
    }

    const navigationExtras: NavigationExtras = { queryParams };

    this.router.navigate([`/tasks/${this.taskId}/all-solutions`], navigationExtras).then(success => {
      if (success) {
        console.log('Route was updated');
      } else {
        console.log('Route was not updated');
      }
    });
  }

  fetchTaskDetails(){
    this.tasksService.getTask(this.taskId).subscribe({
      next: (data) => {
        console.log(data);
        this.taskDetails = data
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.isLoading = false;
        this.errorMessage = "Something went wrong while fetching task details from server... Please, try later!";
      }
    })
  }

  updatePageNumberAndView(value: number){
    this.filterForm.value.pageNumber = value;
    this.fetchSolutions()
  }

  getPaginationBackRange(): number[] {
    let lowest = Math.max(1, this.solutionsViewModel.currentPage - 4);
    let highest = this.solutionsViewModel.currentPage;
    return [...this.rangeService.range(lowest, highest)]
  }

  getPaginationFrontRange(): number[] {
    let lowest = this.solutionsViewModel.currentPage + 1;
    let highest = Math.min(this.solutionsViewModel.totalPages, this.solutionsViewModel.currentPage + 4) + 1;
    return [...this.rangeService.range(lowest, highest)]
  }

  getEditorModeFromLanguageName(){
    let lang = this.taskDetails?.programmingLanguage.name;
    switch (lang){
      case 'FSharp':
        return 'text';
      default:
        return lang?.toLowerCase() ?? "text";
    }
  }
}
