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

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['initialTask']) {
      return;
    }

    const task = this.initialTask;

    if (task) {
      this.form.patchValue({
        title: task.title ?? '',
        description: task.description ?? '',
        status: task.status ?? 'TODO',
        dueDate: this.toDateTimeLocalValue(task.dueDate)
      });
      return;
    }

    this.resetForm();
  }

  get hasTitleError(): boolean {
    const title = (this.form.get('title')?.value as string | null) ?? '';
    return !title.trim();
  }

  get hasPastDueDateError(): boolean {
    const dueDate = (this.form.get('dueDate')?.value as string | null) ?? '';
    const status = (this.form.get('status')?.value as TaskStatus | null) ?? 'TODO';

    if (!dueDate || status === 'DONE') {
      return false;
    }

    return new Date(dueDate).getTime() < new Date().getTime();
  }

  onSubmit(): void {
    if (this.hasTitleError || this.hasPastDueDateError) {
      return;
    }

    const title = ((this.form.get('title')?.value as string | null) ?? '').trim();
    const descriptionValue = ((this.form.get('description')?.value as string | null) ?? '').trim();
    const status = ((this.form.get('status')?.value as TaskStatus | null) ?? 'TODO');
    const dueDate = (this.form.get('dueDate')?.value as string | null) ?? '';

    const payload: TaskCreateOrUpdate = {
      title,
      description: descriptionValue ? descriptionValue : null,
      status,
      dueDate: dueDate ? new Date(dueDate).toISOString() : null
    };

    this.submitTask.emit(payload);

    if (!this.initialTask) {
      this.resetForm();
    }
  }

  onCancel(): void {
    this.cancelEdit.emit();
  }

  private resetForm(): void {
    this.form.reset({
      title: '',
      description: '',
      status: 'TODO',
      dueDate: ''
    });
  }

  private toDateTimeLocalValue(value?: string | null): string {
    if (!value) {
      return '';
    }

    return value.slice(0, 16);
  }
}
