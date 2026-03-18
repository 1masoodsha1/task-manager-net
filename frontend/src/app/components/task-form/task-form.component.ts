import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Task, TaskCreateOrUpdate, TaskStatus } from '../../models/task.model';

@Component({
selector: 'app-task-form',
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
          dueDate: t.dueDate ?? ''
        });
      } else {
        this.form.reset({ title: '', description: '', status: 'TODO', dueDate: '' });
      }
    }
  }

  onSubmit() {
    if (!this.form.valid) return;
    const val = this.form.value;
    const payload: TaskCreateOrUpdate = {
      title: val.title,
      description: val.description || null,
      status: val.status,
      dueDate: val.dueDate || null
    };
    this.submitTask.emit(payload);
  }

  onCancel() {
    this.cancelEdit.emit();
  }
}
