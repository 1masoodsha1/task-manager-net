import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Task, TaskCreateOrUpdate } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api/tasks/`;

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.baseUrl).pipe(catchError(this.handleError));
  }

  getTask(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}${id}/`).pipe(catchError(this.handleError));
  }

  createTask(task: TaskCreateOrUpdate): Observable<Task> {
    return this.http.post<Task>(this.baseUrl, task).pipe(catchError(this.handleError));
  }

  updateTask(id: number, task: TaskCreateOrUpdate): Observable<Task> {
    return this.http.put<Task>(`${this.baseUrl}${id}/`, task).pipe(catchError(this.handleError));
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}${id}/`).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let message = 'Request failed';
    const data = error.error;

    if (typeof data === 'string') {
      message = data;
    } else if (data?.detail) {
      message = String(data.detail);
    } else if (data?.dueDate) {
      message = Array.isArray(data.dueDate) ? data.dueDate.join(', ') : String(data.dueDate);
    } else if (error.message) {
      message = error.message;
    }

    return throwError(() => new Error(message));
  }
}
