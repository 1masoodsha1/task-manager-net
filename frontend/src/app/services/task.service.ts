import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Task, TaskCreateOrUpdate } from '../models/task.model';

@Injectable({
providedIn: 'root'
})
export class TaskService {
private base = `${environment.apiBaseUrl}/api/tasks`;

constructor(private http: HttpClient) {}

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.base);
  }

  getTask(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.base}/${id}`);
  }

  createTask(task: TaskCreateOrUpdate): Observable<Task> {
    return this.http.post<Task>(this.base, task);
  }

  updateTask(id: number, task: TaskCreateOrUpdate): Observable<Task> {
    return this.http.put<Task>(`${this.base}/${id}`, task);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
