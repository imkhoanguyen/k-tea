import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { UserService } from '../../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { RoleService } from '../../../core/services/role.service';
import { Role } from '../../../shared/models/role';
import { Router } from '@angular/router';
import { UserAdd } from '../../../shared/models/user';

@Component({
  selector: 'app-user-add',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
    NzCheckboxModule,
    NzSelectModule,
    FormsModule,
  ],
  templateUrl: './user-add.component.html',
  styleUrl: './user-add.component.css',
})
export class UserAddComponent implements OnInit {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);
  private roleService = inject(RoleService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  roles: Role[] = [];

  ngOnInit(): void {
    this.loadRoles();
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      userName: ['', Validators.required],
      fullName: ['', Validators.required],
      password: ['', Validators.required],
      role: ['', Validators.required],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],
    });
  }

  loadRoles() {
    this.roleService.getAllRoles().subscribe({
      next: (res) => {
        this.roles = res;
      },
      error: (er) => {
        this.toastrService.error(
          'Có lỗi xảy ra khi tải danh sách quyền vui lòng load lại trang'
        );
        console.log(er);
      },
    });
  }

  submitForm() {
    const userAdd: UserAdd = {
      userName: this.frm.value.userName,
      fullName: this.frm.value.fullName,
      phoneNumber: this.frm.value.phoneNumber,
      email: this.frm.value.email,
      password: this.frm.value.password,
      role: this.frm.value.role,
    };

    this.userService.add(userAdd).subscribe({
      next: (res) => {
        this.toastrService.success('Tạo người dùng thành công');
        this.handleCancel();
      },
      error: (er) => {
        this.validationErrors = er;
        console.log(er);
      },
    });
  }

  handleCancel() {
    this.frm.reset();
    this.roles = [];
    this.validationErrors = [];
    this.router.navigateByUrl('/nguoi-dung');
  }
}
