import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { UserService } from '../../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { RoleService } from '../../../core/services/role.service';
import { Role } from '../../../shared/models/role';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { AppUser, UserUpdate } from '../../../shared/models/user';

@Component({
  selector: 'app-user-update',
  standalone: true,
  imports: [
    NzButtonModule,
    NzInputModule,
    NzIconModule,
    CommonModule,
    NzSelectModule,
    ReactiveFormsModule,
    FormsModule,
    NzCardModule,
    NzFormModule,
  ],
  templateUrl: './user-update.component.html',
  styleUrl: './user-update.component.css',
})
export class UserUpdateComponent implements OnInit {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);
  private roleService = inject(RoleService);
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  userName = '';
  roleName = '';

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get('username')!;
    this.loadRoles();
    this.getUser();
    this.initForm();
  }

  roles: Role[] = [];
  passwordVisible = false;

  getUser() {
    this.userService.get(this.userName).subscribe({
      next: (res) => {
        this.roleName = res.role;
        this.patchForm(res);
      },
      error: (er) => {
        this.toastrService.error(
          'Có lỗi xảy ra khi tải thông tin người dùng. Vui lòng load lại trang'
        );
        console.log(er);
      },
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

  validationChangeRole?: string[];
  changeRole() {
    console.log(this.roleName);
    this.userService.changeRole(this.userName, this.roleName).subscribe({
      next: (_) => {
        this.toastrService.success('Thay đổi quyền thành công');
        this.reset();
      },
      error: (er) => {
        console.log(er);
        this.validationChangeRole = er;
      },
    });
  }

  backUserListPage() {
    this.router.navigateByUrl('/nguoi-dung');
  }

  // update info user
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];

  initForm() {
    this.frm = this.fb.group({
      userName: ['', Validators.required],
      fullName: ['', Validators.required],
      password: [''],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      address: ['', Validators.required],
    });
  }

  submitForm() {
    const update: UserUpdate = {
      userName: this.frm.value.userName,
      fullName: this.frm.value.fullName,
      phoneNumber: this.frm.value.phoneNumber,
      email: this.frm.value.email,
      password: this.frm.value.password,
      address: this.frm.value.address,
    };

    this.userService.update(update).subscribe({
      next: (res) => {
        this.toastrService.success('Cập nhật thành công');
        this.reset();
      },
      error: (er) => {
        this.validationErrors = er;
      },
    });
  }

  patchForm(user: AppUser) {
    this.frm.patchValue({
      userName: user.userName,
      fullName: user.fullName,
      phoneNumber: user.phoneNumber,
      email: user.email,
      address: user.address,
    });
  }

  reset() {
    this.validationChangeRole = [];
    this.validationErrors = [];
    this.loadRoles();
    this.getUser();
  }
}
