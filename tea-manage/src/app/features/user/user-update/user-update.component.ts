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
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

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
  isLooked = false;

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get('username')!;
    this.loadRoles();
    this.getUser();
  }

  roles: Role[] = [];
  passwordVisible = false;

  getUser() {
    this.userService.get(this.userName).subscribe({
      next: (res) => {
        this.roleName = res.role;
        this.isLooked = res.isLoocked;
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
        // if (this.userName === this.authService.currentUser()?.userName) {
        //   this.authService
        //     .callRefreshToken(
        //       this.authService.currentUser()?.refreshToken ?? ''
        //     )
        //     .subscribe({
        //       next: (res) => {
        //         this.authService.setCurrentUser(res);
        //       },
        //       error: (er) => {
        //         console.log(er);
        //       },
        //     });
        // }
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
}
