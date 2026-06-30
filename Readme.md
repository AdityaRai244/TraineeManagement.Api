## Project Name
Trainee Management System
 
## Technology Used
Asp.net core, MYSql, Redis, Rabbit MQ, Docker


## Prerequisites
Create an appsettings.json file in root of your directory. Copy the appsettings.example.json file into the newly created appsettings.json file. Replace the variable values with the actual values. For eg : {SERVER_NAME} --> localhost, {MYSQL_USERNAME} --> root. etc.

## Architecture Diagram
https://excalidraw.com/#json=2nuc-iK5WIf8_s6zKylg8,_Ps5B0OLzFsHI52FZ7iUuw


## Backend setup steps

  ### TraineeManagement.Api
  1. Navigate to the TraineeManagement.Api folder  
    From the root of the project run:  
      ```javascript
        cd TraineeManagement.Api
      ```

  2. Set up appsettings.json files  
    Create a new file called appsettings.json. Copy paste the content from appsettings.example.json to the newly created file and replace with your actual values. 

  3. Restore dependencies  
      ```javascript
        dotnet restore
      ``` 

  4. Build the project   
      ```javascript
        dotnet build
      ```

  6. Run the application  
      ```javascript
        dotnet run
      ```


  ### WorkerService
  1. Navigate to the WorkerService folder    
    From the root of the project run:   
      ```javascript 
        cd WorkerService
      ```

  2. Set up appsettings.json files  
    Create a new file called appsettings.json. Copy paste the content from appsettings.example.json to the newly created file and replace with your actual values. 
  
  3. Restore dependencies  
      ```javascript 
        dotnet restore
      ```

  4. Build the project   
      ```javascript 
        dotnet build
      ```

  5. Run the application  
      ```javascript 
        dotnet run
      ```

## MySQL setup steps
1. Get a database connection string

2. Add Database connection string in the appsettings.json file of TraineeManagement.Api and WorkerService 

3. Run the following command in the root of the project dir to make sure there are no errors.  
  ```javascript 
    dotnet build 
  ```

4. Run the following command in the root of the project dir to create tables in the database   
  ```javascript 
    dotnet ef database update -p Shared -s TraineeManagement.Api
  ```

Once ran successfully, the API and the database are in sync. We can test the connection by using swagger UI, try adding one entry  using POST end point and see if it is shown in the datase or not.

## Redis setup steps
1. Get a redis connection string and ensure your redis instance is up and running

2. Add Redis connection string in the appsettings.json file of TraineeManagement.Api and WorkerService



## RabbitMQ setup steps
1. Ensure a rabbitMQ instance is up and running

2. Add RabbitMQ username in the appsettings.json file of TraineeManagement.Api and WorkerServices 

3. Add RabbitMQ password in the appsettings.json file of TraineeManagement.Api and WorkerServices 


## Login credentials for testing
``` javascript
{
  "username": "string",
  "password": "string"
}
```

## JWT usage instructions
1. Generate and use a random string as your Jwt SecretKey

2. Add Jwt SecretKey to appsettings.json file in the TraineeManagement.Api folder  


## Setup using Docker
1. Ensure the appsettings.json files in TraineeManagement.Api and WorkerService are appropriately filled

2. Run the following command in the root of the project directory to start the application using docker:  
  ``` javascript
    docker compose up --build -d
  ```

# API List
```
GET    /api/health 
POST   /api/auth/login 
GET    /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active 
GET    /api/trainees/{id} 
POST   /api/trainees 
PUT    /api/trainees/{id} 
DELETE /api/trainees/{id} 
GET    /api/mentors 
GET    /api/mentors/{id} 
POST   /api/mentors 
PUT    /api/mentors/{id} 
DELETE /api/mentors/{id} 
GET    /api/learning-tasks 
GET    /api/learning-tasks/{id} 
POST   /api/learning-tasks 
PUT    /api/learning-tasks/{id} 
DELETE /api/learning-tasks/{id} 
POST   /api/task-assignments 
GET    /api/task-assignments 
GET    /api/task-assignments/{id} 
PUT    /api/task-assignments/{id}/status 
POST   /api/submissions 
GET    /api/submissions 
GET    /api/submissions/{id} 
POST   /api/reviews 
GET    /api/reviews 
GET    /api/reviews/{id} 
```

 
## Sample Request JSON
```json
Sample POST and PUT /api/trainees request:
{
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Active"
}
{
  "firstName": "Aditya",
  "lastName": "Rai",
  "email": "aditya.rai@gmail.com",
  "techStack": "JavaScript, Typescript",
  "status": "Inactive"
}
```
 
 
## Sample Response JSON
```json
Sample GET /api/health response:
{
  "status": "running",
  "application": "Trainee Management API",
  "timestamp": "2026-06-10T06:38:09.9985091+00:00"
}
 
Sample GET /api/trainees response:
[
  {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-10T06:38:58.0911902+00:00",
    "updatedDate": "2026-06-10T06:38:58.0912088+00:00"
  },
  {
    "id": 2,
    "firstName": "Aditya",
    "lastName": "Rai",
    "email": "aditya.rai@gmail.com",
    "techStack": "JavaScript, Typescript",
    "status": "InActive",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
  }
]

Sample GET /api/trainees?search={search} response : 
Search term : Aditya
[
  {
    "id": 2,
    "firstName": "Aditya",
    "lastName": "Rai",
    "email": "aditya.rai@gmail.com",
    "techStack": "HTML,JavaScript, Typescript",
    "status": "InActive",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:41:36.8993693+00:00"
  }
]
 
Sample POST /api/trainees response:

{
  "id": 2,
  "firstName": "Aditya",
  "lastName": "Rai",
  "email": "aditya.rai@gmail.com",
  "techStack": "JavaScript, Typescript",
  "status": "InActive",
  "createdDate": "2026-06-10T06:40:04.676972+00:00",
  "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
}

 
Sample GET /api/trainees/{id} response:
{
  "id": 2,
  "firstName": "Aditya",
  "lastName": "Rai",
  "email": "aditya.rai@gmail.com",
  "techStack": "JavaScript, Typescript",
  "status": "InActive",
  "createdDate": "2026-06-10T06:40:04.676972+00:00",
  "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
}

 
Sample PUT /api/trainees/{id} response:
{
  "id": 2,
  "firstName": "Aditya",
  "lastName": "Rai",
  "email": "aditya.rai@gmail.com",
  "techStack": "HTML,JavaScript, Typescript",
  "status": "InActive",
  "createdDate": "2026-06-10T06:40:04.676972+00:00",
  "updatedDate": "2026-06-10T06:41:36.8993693+00:00"
}

Sample POST /api/mentors response : 
{
  "id": 2,
  "firstName": "Mentor FN",
  "lastName": "Mentor LN",
  "email": "mentor@gmail.com",
  "expertise": "REACT",
  "status": "Active",
  "createdDate": "2026-06-15T06:40:53.3100551+00:00",
  "updatedDate": "2026-06-15T06:40:53.3100722+00:00"
}

Sample GET  /api/mentors response : 
[
  {
    "id": 2,
    "firstName": "Mentor FN",
    "lastName": "Mentor LN",
    "email": "mentor@gmail.com",
    "expertise": "REACT",
    "status": "Active",
    "createdDate": "2026-06-15T06:40:53.310055",
    "updatedDate": "2026-06-15T06:40:53.310072"
  }
]

Sample GET /api/mentors/{id} response : 
{
  "id": 2,
  "firstName": "Mentor FN",
  "lastName": "Mentor LN",
  "email": "mentor@gmail.com",
  "expertise": "REACT",
  "status": "Active",
  "createdDate": "2026-06-15T06:40:53.310055",
  "updatedDate": "2026-06-15T06:40:53.310072"
}

Sample PUT  /api/mentors/{id} response :
{
  "id": 2,
  "firstName": "MENTOR",
  "lastName": "LN",
  "email": "mentor@ln.com",
  "expertise": "JAVA",
  "status": "InActive",
  "createdDate": "2026-06-15T06:40:53.310055",
  "updatedDate": "2026-06-15T06:43:35.8565209+00:00"
}

Sample DELETE /api/mentors/{id} response :

Code	Details
204
Undocumented
Response headers
 date: Mon,15 Jun 2026 06:43:59 GMT 
 server: Kestrel 

Sample POST   /api/learning-tasks response :
{
  "id": 2,
  "title": "React APP",
  "description": "Build a react app",
  "expectedTechStack": "REACT",
  "dueDate": "2026-06-15T06:47:16.338Z",
  "status": "Draft",
  "createdDate": "2026-06-15T06:47:32.2228732+00:00",
  "updatedDate": "2026-06-15T06:47:32.2228876+00:00"
}

Sample GET  /api/learning-tasks  response :
[
  {
    "id": 1,
    "title": "string",
    "description": "string",
    "expectedTechStack": "string",
    "dueDate": "2026-06-12T09:29:24.879",
    "status": "Draft",
    "createdDate": "2026-06-12T09:29:27.374844",
    "updatedDate": "2026-06-12T09:29:27.374877"
  },
  {
    "id": 2,
    "title": "React APP",
    "description": "Build a react app",
    "expectedTechStack": "REACT",
    "dueDate": "2026-06-15T06:47:16.338",
    "status": "Draft",
    "createdDate": "2026-06-15T06:47:32.222873",
    "updatedDate": "2026-06-15T06:47:32.222887"
  }
]

Sample GET  /api/learning-tasks/{id} response :
{
    "id": 2,
    "title": "React APP",
    "description": "Build a react app",
    "expectedTechStack": "REACT",
    "dueDate": "2026-06-15T06:47:16.338",
    "status": "Draft",
    "createdDate": "2026-06-15T06:47:32.222873",
    "updatedDate": "2026-06-15T06:47:32.222887"
  }


Sample PUT  /api/learning-tasks/{id} response : 
{
  "id": 1,
  "title": "Weather APP",
  "description": "Bulid weather app",
  "expectedTechStack": "JAVASCRIPT",
  "dueDate": "2026-06-15T06:49:22.464Z",
  "status": "Draft",
  "createdDate": "2026-06-12T09:29:27.374844",
  "updatedDate": "2026-06-15T06:49:44.0697249+00:00"
}

Sample DELETE /api/learning-tasks/{id} response : 

Code	Details
204
Undocumented
Response headers
 date: Mon,15 Jun 2026 06:50:08 GMT 
 server: Kestrel 


Sample POST /api/task-assignments response :
{
  "id": 4,
  "traineeId": 1,
  "mentorId": 3,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T06:51:46.52Z",
  "dueDate": "2026-06-15T06:51:46.52Z",
  "status": "Assigned",
  "remarks": "string"
}

Sample GET  /api/task-assignments response :
[
  {
    "id": 4,
    "traineeId": 1,
    "mentorId": 3,
    "learningTaskId": 1,
    "assignedDate": "2026-06-15T06:51:46.52",
    "dueDate": "2026-06-15T06:51:46.52",
    "status": "Assigned",
    "remarks": "string"
  }
]

Sample GET /api/task-assignments/{id}
{
  "id": 4,
  "traineeId": 1,
  "mentorId": 3,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T06:51:46.52Z",
  "dueDate": "2026-06-15T06:51:46.52Z",
  "status": "Assigned",
  "remarks": "string"
}

Sample PUT /api/task-assignments/{id}/status
{
  "id": 4,
  "traineeId": 1,
  "mentorId": 3,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T06:51:46.52",
  "dueDate": "2026-06-15T06:51:46.52",
  "status": "Submitted",
  "remarks": "string"
}

Sample POST /api/submissions response :  
{
  "id": 3,
  "taskAssignmentId": 4,
  "submissionUrl": "Github",
  "notes": "Upload",
  "submittedDate": "2026-06-15T07:11:54.474Z",
  "status": "Submitted",
  "taskAssignment": null
}

Sample GET /api/submissions response :
[
  {
  "id": 3,
  "taskAssignmentId": 4,
  "submissionUrl": "Github",
  "notes": "Upload",
  "submittedDate": "2026-06-15T07:11:54.474Z",
  "status": "Submitted",
  "taskAssignment": null
}
]
Sample : GET  /api/submissions/{id} response : 
{
  "id": 3,
  "taskAssignmentId": 4,
  "submissionUrl": "Github",
  "notes": "Upload",
  "submittedDate": "2026-06-15T07:11:54.474Z",
  "status": "Submitted",
  "taskAssignment": null
}


Sample : POST   /api/reviews response : 
{
  "id": 2,
  "submissionId": 1,
  "mentorId": 3,
  "feedback": "Good job",
  "score": 20,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T07:12:25.369Z"
}

Sample : GET    /api/reviews response : 
[
  {
  "id": 2,
  "submissionId": 1,
  "mentorId": 3,
  "feedback": "Good job",
  "score": 20,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T07:12:25.369Z"
}
]
Sample GET    /api/reviews/{id} resposne :
{
  "id": 2,
  "submissionId": 1,
  "mentorId": 3,
  "feedback": "Good job",
  "score": 20,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T07:12:25.369Z"
}
```

## Known Limitations
- Using In-memory database instead of Sql or NoSql database
 

