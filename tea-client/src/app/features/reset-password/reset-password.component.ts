import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { ResetPassword } from '../../shared/models/user';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    NzInputModule,
    NzCardModule,
    NzButtonModule,
    NzIconModule,
  ],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent implements OnInit {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  private resetPassword: ResetPassword = {
    email: '',
    password: '',
    token: '',
  };

  passwordVisible = false;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.resetPassword.email = params['email'];
      this.resetPassword.token = params['token'];
    });
  }
  password: string = '';
  confirmPassword: string = '';
  passwordErrorMessage: string = '';
  confirmPasswordErrorMessage: string = '';

  onSubmit() {
    this.passwordErrorMessage = '';
    this.confirmPasswordErrorMessage = '';

    if (!this.password) {
      this.passwordErrorMessage = 'Mật khẩu không được để trống';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.confirmPasswordErrorMessage = 'Mật khẩu xác nhận không khớp!';
      return;
    }

    this.resetPassword.password = this.password;
    this.userService.resetPassword(this.resetPassword).subscribe({
      next: (response) => {
        this.toastrService.success((response as any).message);
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (er) => {
        console.log(er);
        if (er.error === 'Invalid token.') {
          this.toastrService.error(er.error);
        } else {
          this.passwordErrorMessage = er.error;
        }
      },
    });
  }
}
