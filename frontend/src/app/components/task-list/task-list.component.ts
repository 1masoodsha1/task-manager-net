import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Task } from '../../models/task.model';

@Component({
selector: 'app-task-list',
templateUrl: './task-list.component.html',
styleUrls: ['./task-list.component.css']
})
export class TaskListComponent {
@Input() tasks: Task[] = [];
@Input() loading = false;

@Output() edit = new EventEmitter<Task>();
@Output() delete = new EventEmitter<number>();
@Output() refresh = new EventEmitter<void>();

onEdit(t: Task) { this.edit.emit(t); }
  onDelete(id: number) {
    if (confirm('Delete task?')) this.delete.emit(id);
  }
  onRefresh() { this.refresh.emit(); }
}
