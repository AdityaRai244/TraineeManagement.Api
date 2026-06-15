## Project Name
Trainee Management API
 
## Technology Used
Asp.net core


## Prerequisites
Create an appsettings.json file in root of your directory. Copy the appsettings.example.json file into the newly created appsettings.json file. Replace the variable values with the actual values. For eg : {SERVER_NAME} --> localhost, {MYSQL_USERNAME} --> root. etc.

## DB SETUP

 
## How to Run
run `dotnet run` in the root of the project directory

# Login Credentials for testing
Username : admin
Password : admin

# MySQL Setup Commands (WSL/Ubuntu)
 
## 1. Update Ubuntu Packages
 
```bash
sudo apt update
````
 
***
 
## 2. Install MySQL Server
 
```bash
sudo apt install mysql-server -y
```
 
***
 
## 3. Start MySQL Service
 
```bash
sudo service mysql start
```
 
***
 
## 4. Check MySQL Status
 
```bash
sudo service mysql status
```
 
Expected:
 
```text
active (running)
```
 
***
 
## 5. Open MySQL as sudo User
 
```bash
sudo mysql
```
 
***
 
## 6. Change Root Authentication to Password-Based Login
 
```sql
ALTER USER 'root'@'localhost'
IDENTIFIED WITH mysql_native_password
BY 'Root@123';
```
 
***
 
## 7. Apply Changes
 
```sql
FLUSH PRIVILEGES;
```
 
***
 
## 8. Verify Authentication Plugin
 
```sql
SELECT user, host, plugin FROM mysql.user;
```
 
Expected:
 
```text
root | localhost | mysql_native_password
```
 
***
 
## 9. Exit MySQL
 
```sql
exit;
```
 
***
 
## 10. Restart MySQL
 
```bash
sudo service mysql restart
```
 
***
 
## 11. Login Using Root Password
 
```bash
mysql -u root -p
```
 
Password:
 
```text
Root@123
```
 
***
 
## 12. Create Database
 
```sql
CREATE DATABASE trainee_management_db;
```
 
***
 
## 13. Verify Database
 
```sql
SHOW DATABASES;
```
 
Expected:
 
```text
trainee_management_db
```
 
***
 
## 14. Exit MySQL
 
```sql
exit;
```
 
***
 
# EF Core + MySQL Setup Commands
 
## 1. Remove Old InMemory Package
 
```bash
dotnet remove package Microsoft.EntityFrameworkCore.InMemory
```
 
***
 
## 2. Install EF Core MySQL Packages
 
```bash
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
```
 
***
 
## 3. Restore Packages
 
```bash
dotnet restore
```
 
***
 
## 4. Install dotnet ef Tool
 
```bash
dotnet tool install --global dotnet-ef
```
 
***
 
## 5. Add dotnet Tools Path
 
```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```
 
***
 
## 6. Create Migration
 
```bash
dotnet ef migrations add InitialCreate
```
 
***
 
## 7. Apply Migration
 
```bash
dotnet ef database update
```
 
***
 
## 8. Run Application
 
```bash
dotnet run
```
 
***
 
# Verify Tables in MySQL
 
Login:
 
```bash
mysql -u root -p
```
 
Select database:
 
```sql
USE trainee_management_db;
```
 
Show tables:
 
```sql
SHOW TABLES;
```
 
Expected:
 
```text
Trainees
__EFMigrationsHistory
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
 

