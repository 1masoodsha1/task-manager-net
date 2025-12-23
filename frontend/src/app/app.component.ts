import { Component, OnInit } from '@angular/core';
import { TaskService } from './services/task.service';
import { Task, TaskCreateOrUpdate } from './models/task.model';

@Component({
selector: 'app-root',
templateUrl: './app.component.html',
styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
tasks: Task[] = [];
loading = false;
editingTask: Task | null = null;
error: string | null = null;

constructor(private svc: TaskService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.loading = true;
    this.error = null;
    this.svc.getTasks().subscribe({
      next: data => { this.tasks = data; this.loading = false; },
      error: err => { this.error = err?.message ?? 'Failed to load tasks'; this.loading = false; }
    });
  }

  handleCreateOrUpdate(taskInput: TaskCreateOrUpdate) {
    this.error = null;
    if (this.editingTask) {
      this.svc.updateTask(this.editingTask.id, taskInput).subscribe({
        next: updated => {
          this.tasks = this.tasks.map(t => t.id === updated.id ? updated : t);
          this.editingTask = null;
        },
        error: err => this.error = err?.message ?? 'Failed to save task'
      });
    } else {
      this.svc.createTask(taskInput).subscribe({
        next: created => this.tasks = [...this.tasks, created],
        error: err => this.error = err?.message ?? 'Failed to save task'
      });
    }
  }

  onEdit(t: Task) { this.editingTask = t; }
  onDelete(id: number) {
    this.error = null;
    this.svc.deleteTask(id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(t => t.id !== id);
        if (this.editingTask && this.editingTask.id === id) this.editingTask = null;
      },
      error: err => this.error = err?.message ?? 'Failed to delete task'
    });
  }

  onRefresh() { this.loadTasks(); }
  onCancelEdit() { this.editingTask = null; }
}
