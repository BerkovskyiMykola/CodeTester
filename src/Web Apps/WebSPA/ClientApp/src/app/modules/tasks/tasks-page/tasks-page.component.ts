import {Component, OnInit} from '@angular/core';
import {IPageView} from "../../shared/models/general/page-view.model";
import {TasksService} from "../tasks.service";
import {IGetTasksRequest} from "../../shared/models/tasks/get-tasks-request.model";
import {FormControl, FormGroup} from "@angular/forms";
import {DictionaryService} from "../dictionary.service";
import {IDifficulty} from "../../shared/models/dictionary/difficulty.model";
import {ITaskType} from "../../shared/models/dictionary/task-type.model";
import {IProgrammingLanguage} from "../../shared/models/dictionary/programming-language";
import {ActivatedRoute, NavigationExtras, Params, Router} from "@angular/router";
import {ITaskDetails} from "../../shared/models/tasks/task-details.model";
import {RangeService} from "../../shared/services/range.service";

@Component({
  selector: 'app-tasks-page',
  templateUrl: './tasks-page.component.html',
  styleUrls: ['./tasks-page.component.css']
})
export class TasksPageComponent implements OnInit{
  constructor(
    private tasksService: TasksService,
    private dictionaryService: DictionaryService,
    private router: Router,
    private route: ActivatedRoute,
    private rangeService: RangeService
  ) {
  }

  tasksViewModel: IPageView<ITaskDetails> = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 10,
    totalCount: 0,
    hasPrevious: false,
    hasNext: true,
    rows: []
  };
  errorMessage = "";
  isError = false;
  isFilterBlockShown = false;
  isLoading = false;
  isEmptyResult = false;

  difficulties: IDifficulty[] = []
  taskTypes: ITaskType[] = []
  programmingLanguages: IProgrammingLanguage[] = []

  filterForm = new FormGroup({
    search: new FormControl<string>(''),
    difficultyId: new FormControl<number | undefined>(undefined),
    programmingLanguageId: new FormControl<number | undefined>(undefined),
    typeId: new FormControl<number | undefined>(undefined),
    isCompleted: new FormControl<boolean | undefined>(undefined),
    pageNumber: new FormControl<number>(1),
    pageSize: new FormControl<number>(10),
  })
  pageCountVariants = [1, 2, 5, 10, 25]

  ngOnInit(): void {
    this.isLoading = true;

    this.route.queryParams
      .subscribe(params => {
        this.writeParamsToFormGroup(params)
      })

    this.updateTasksList()

    this.fetchDifficulties()
    this.fetchTaskTypes()
    this.fetchProgrammingLanguages()
  }

  writeParamsToFormGroup(params: Params){
    const search = params.search;
    const difficultyId = params.difficultyId ? parseInt(params.difficultyId) : null;
    const programmingLanguageId = params.programmingLanguageId ? parseInt(params.programmingLanguageId) : null;
    const typeId = params.typeId ? parseInt(params.typeId) : null;
    const isCompleted = params.isCompleted !== undefined ? params.isCompleted : null;
    const pageNumber = params.pageNumber ? parseInt(params.pageNumber) : 1;
    const pageSize = params.pageSize ? parseInt(params.pageSize) : 10;

    const model = {
      search,
      difficultyId,
      programmingLanguageId,
      typeId,
      isCompleted,
      pageNumber,
      pageSize
    }

    this.filterForm.patchValue(model);
    console.log("filterForm: ", this.filterForm.value)
  }

  fetchDifficulties(){
    this.dictionaryService.getDifficulties().subscribe({
      next: (data) => {
        this.difficulties = data;
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.errorMessage = "Something went wrong while fetching data from server... Please, try later!";
      }
    })
  }

  fetchProgrammingLanguages(){
    this.dictionaryService.getProgrammingLanguages().subscribe({
      next: (data) => {
        this.programmingLanguages = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.isLoading = false;
        this.errorMessage = "Something went wrong while fetching data from server... Please, try later!";
      }
    })
  }

  fetchTaskTypes(){
    this.dictionaryService.getTaskTypes().subscribe({
      next: (data) => {
        this.taskTypes = data;
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.errorMessage = "Something went wrong while fetching data from server... Please, try later!";
      }
    })
  }

  clearFilters(){
    this.filterForm.reset({
      search: this.filterForm.value.search,
      pageSize: this.filterForm.value.pageSize
    })
    this.updateTasksList()
  }


  getRequestModel(){
    const search = this.filterForm.value.search;
    const difficultyId = this.filterForm.value.difficultyId;
    const programmingLanguageId = this.filterForm.value.programmingLanguageId;
    const typeId = this.filterForm.value.typeId;
    const isCompleted = this.filterForm.value.isCompleted;
    const pageNumber = this.filterForm.value.pageNumber;
    const pageSize = this.filterForm.value.pageSize;

    return {
      ...(search && search.trim().length > 0 && {search: search}),
      ...(difficultyId && {difficultyId: difficultyId}),
      ...(programmingLanguageId && {programmingLanguageId: programmingLanguageId}),
      ...(typeId && {typeId: typeId}),
      ...(isCompleted !== undefined && isCompleted !== null && {isCompleted: isCompleted as boolean}),
      ...(pageNumber && {pageNumber: pageNumber}),
      ...(pageSize && {pageSize: pageSize})
    };
  }

  checkForEmptyResult(){
    if (this.tasksViewModel.rows.length === 0){
      this.isEmptyResult = true;
      return;
    }
    this.isEmptyResult = false;
  }

  updateTasksList(){
    //make model
    const requestModel: IGetTasksRequest = this.getRequestModel();

    console.log(requestModel)
    //fetch data
    this.tasksService.getTasksList(requestModel).subscribe({
      next: (data) => {
        console.log(data);
        this.tasksViewModel = data;
        this.checkForEmptyResult()
        this.isError = false;

        console.log("Final:", this.filterForm.value)
      },
      error: (err) => {
        console.log(err);
        this.isError = true;
        this.errorMessage = "Something went wrong... Please, try later!";
      }
    })

    //change queries in link
    this.updateRouteWithParams(requestModel)
  }

  updateRouteWithParams(requestModel: IGetTasksRequest){
    const queryParams: { [key: string]: any } = {};
    for (const [key, value] of Object.entries(requestModel)) {
      queryParams[key] = value;
    }

    const navigationExtras: NavigationExtras = { queryParams };

    this.router.navigate(['/tasks'], navigationExtras).then(success => {
      if (success) {
        console.log('Route was updated');
      } else {
        console.log('Route was not updated');
      }
    });
  }

  updatePageNumberAndView(value: number){
    this.filterForm.value.pageNumber = value;
    this.updateTasksList()
  }

  getPaginationBackRange(): number[] {
    let lowest = Math.max(1, this.tasksViewModel.currentPage - 4);
    let highest = this.tasksViewModel.currentPage;
    return [...this.rangeService.range(lowest, highest)]
  }

  getPaginationFrontRange(): number[] {
    let lowest = this.tasksViewModel.currentPage + 1;
    let highest = Math.min(this.tasksViewModel.totalPages, this.tasksViewModel.currentPage + 4) + 1;
    return [...this.rangeService.range(lowest, highest)]
  }
}
