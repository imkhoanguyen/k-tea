import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { AppUser, UserUpdate } from '../../shared/models/user';

@Component({
  selector: 'app-update-info',
  standalone: true,
  imports: [
    NzButtonModule,
    NzInputModule,
    NzIconModule,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NzFormModule,
  ],
  templateUrl: './update-info.component.html',
  styleUrl: './update-info.component.css',
})
export class UpdateInfoComponent {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);
  private activatedRoute = inject(ActivatedRoute);
  userName = '';

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get('username')!;
    this.getUser();
    this.initForm();
  }

  getUser() {
    this.userService.get(this.userName).subscribe({
      next: (res) => {
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
        if (this.userName === this.userService.currentUser()?.userName) {
          this.userService
            .callRefreshToken(
              this.userService.currentUser()?.refreshToken || ''
            )
            .subscribe({
              next: (res) => {
                this.userService.setCurrentUser(res);
              },
              error: (er) => {
                console.log(er);
              },
            });
        }
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
    this.validationErrors = [];
    this.getUser();
  }
}
