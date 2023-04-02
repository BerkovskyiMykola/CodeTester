export interface ISolution {
  id: string,
  code: string
  success: boolean,
  createDate: Date,
  user: {
    id: string,
    fullname: string
  }
}
