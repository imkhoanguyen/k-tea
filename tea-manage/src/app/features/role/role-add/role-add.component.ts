import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { RoleService } from '../../../core/services/role.service';
import { ToastrService } from 'ngx-toastr';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { Router } from '@angular/router';
import { Role, RoleAdd } from '../../../shared/models/role';

@Component({
  selector: 'app-role-add',
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
  templateUrl: './role-add.component.html',
  styleUrl: './role-add.component.css',
})
export class RoleAddComponent {
  private roleService = inject(RoleService);
  private toastrService = inject(ToastrService);
  @Output() roleAdded = new EventEmitter<Role>();
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  isVisible = false;

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
    const roleAdd: RoleAdd = {
      name: this.frm.value.name,
      description: this.frm.value.description,
    };

    this.roleService.add(roleAdd).subscribe({
      next: (res) => {
        this.toastrService.success('Thêm quyền thành công');
        this.roleAdded.emit(res as Role);
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

  showModal(): void {
    this.isVisible = true;
  }
}
