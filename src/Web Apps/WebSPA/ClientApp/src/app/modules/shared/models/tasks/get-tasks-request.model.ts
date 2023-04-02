export interface IGetTasksRequest{
  search?: string,
  difficultyId?: number,
  programmingLanguageId?: number,
  typeId?: number,
  isCompleted?: boolean
  pageNumber?: number,
  pageSize?: number
}
