import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Role, RoleUpdate } from '../../../shared/models/role';
import { RoleService } from '../../../core/services/role.service';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-update',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
    FormsModule,
  ],
  templateUrl: './role-update.component.html',
  styleUrl: './role-update.component.css',
})
export class RoleUpdateComponent implements OnInit {
  private roleService = inject(RoleService);
  private toastrService = inject(ToastrService);
  @Output() roleUpdated = new EventEmitter<Role>();
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  isVisible = false;
  id: string = '';

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  submitForm() {
    const roleUpdate: RoleUpdate = {
      id: this.id,
      name: this.frm.value.name,
      description: this.frm.value.description,
    };

    this.roleService.update(roleUpdate).subscribe({
      next: (res) => {
        this.toastrService.success('Cập nhật quyền thành công');
        this.roleUpdated.emit(res as Role);
        this.handleCancel();
      },
      error: (er) => {
        this.validationErrors = er;
        console.log(er);
      },
    });
  }

  handleCancel() {
    this.isVisible = false;
    this.frm.reset();
    this.validationErrors = [];
  }

  pathValue(role: Role) {
    this.id = role.id;
    this.frm.patchValue({
      name: role.name,
      description: role.description,
    });
  }

  showModal(): void {
    if (this.id !== '') this.loadRole();
    else this.toastrService.error('Vui lòng chọn lại quyền cần sửa');
  }

  loadRole() {
    this.roleService.get(this.id).subscribe({
      next: (res) => {
        this.isVisible = true;
        this.pathValue(res as Role);
      },
      error: (er) => {
        console.log(er);
        this.handleCancel();
      },
    });
  }
}
