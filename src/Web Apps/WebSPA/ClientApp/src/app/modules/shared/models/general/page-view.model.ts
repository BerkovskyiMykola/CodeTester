export interface IPageView<T>{
  currentPage: number,
  totalPages: number,
  pageSize: number,
  totalCount: number,
  hasPrevious: boolean,
  hasNext: boolean,
  rows: T[]
}
