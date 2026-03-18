## Welcome 👋

Thanks for taking the time to work through this exercise!

We don’t expect perfection or a “finished product” in the short time you have. The goal is simply to see how you think,
how you structure code, and how you approach a realistic problem. It’s completely okay if you don’t get through everything.

A few things to keep in mind while you work:

- You’re encouraged to make reasonable assumptions if something isn’t fully specified.
- There isn’t one “right” solution — we’re more interested in your reasoning than in a specific pattern or framework.
- Feel free to leave comments or notes in the code if you’d like to explain trade-offs or what you’d do with more time.

Above all, relax and have fun with it. Treat this as a chance to show how you naturally work on a small but real-world backend feature rather than an exam.

---
## 🚀 Setup Instructions

This project contains two separate applications:

- **Backend** — ASP.NET Core Web API 
- **Frontend** — Angular 12 

You must run **both** for the application to work.

---

## 📦 Setup

### **Requirements**
BE
- Port **5000** must be free

FE
- NPM (or Yarn)
- Port 4200 must be available 

### **Steps**

BE
1. Navigate to the backend directory:

   ```bash
   cd TaskManagerBackend
   ```
2. Start the .NET application:

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
FE
1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```
   npm start 
   ```
---

### Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core** (SQLite for local dev)
- Swagger UI for API exploration

---

## Current API Overview

The backend exposes a simple REST API for managing tasks under the base path:

```text
GET    /api/tasks            → list all tasks
GET    /api/tasks/{id}       → return task by id (404 if not found)
POST   /api/tasks            → create a new task, returns created resource (201)
PUT    /api/tasks/{id}       → update an existing task
DELETE /api/tasks/{id}       → delete a task (204 on success)
```