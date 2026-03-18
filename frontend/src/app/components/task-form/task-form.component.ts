import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Task, TaskCreateOrUpdate, TaskStatus } from '../../models/task.model';

@Component({
selector: 'app-task-form',
standalone: true,
imports: [CommonModule, ReactiveFormsModule],
templateUrl: './task-form.component.html',
styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent implements OnChanges {
@Input() initialTask: Task | null = null;
@Output() submitTask = new EventEmitter<TaskCreateOrUpdate>();
@Output() cancelEdit = new EventEmitter<void>();

form: FormGroup;
statuses: TaskStatus[] = ['TODO', 'IN_PROGRESS', 'DONE'];

constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      status: ['TODO', Validators.required],
      dueDate: ['']
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.initialTask) {
      const t: Task | null = this.initialTask;

      if (t) {
        this.form.patchValue({
          title: t.title,
          description: t.description ?? '',
          status: t.status,
          dueDate: this.toDateTimeLocalValue(t.dueDate)
        });
      } else {
        this.form.reset({
          title: '',
          description: '',
          status: 'TODO',
          dueDate: ''
        });
      }
    }
  }

  get hasPastDueDateError(): boolean {
    const dueDate = this.form.get('dueDate')?.value as string;
    const status = this.form.get('status')?.value as TaskStatus;

    if (!dueDate || status === 'DONE') return false;

    const selected = new Date(dueDate);
    const now = new Date();

    return selected.getTime() < now.getTime();
  }

  onSubmit() {
    if (!this.form.valid || this.hasPastDueDateError) return;

    const val = this.form.value;
    const payload: TaskCreateOrUpdate = {
      title: val.title,
      description: val.description || null,
      status: val.status,
      dueDate: val.dueDate ? new Date(val.dueDate).toISOString() : null
    };

    this.submitTask.emit(payload);
  }

  onCancel() {
    this.cancelEdit.emit();
  }

  private toDateTimeLocalValue(value?: string | null): string {
    if (!value) return '';

    const date = new Date(value);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
