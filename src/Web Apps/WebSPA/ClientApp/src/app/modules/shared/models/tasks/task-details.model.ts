export interface ITaskDetails{
  id: string,
  title: string,
  description: {
    text: string,
    examples: string,
    someCases: string,
    note: string
  },
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
  "solutionTemplate": string,
  "completedAmount": number,
  "isCompleted": boolean
}
