export interface ITaskCard{
  "id": string,
  "title": string,
  "difficulty": {
    "id": number,
    "name": string
  },
  "taskType": {
    "id": number,
    "name": string
  },
  "programmingLanguage": {
    "id": number,
    "name": string
  },
  "completedAmount": number,
  "isCompleted": boolean
}
