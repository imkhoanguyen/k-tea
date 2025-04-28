import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, NzInputModule, NzButtonModule, NzCardModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);

  email: string = '';
  errorMessage: string = '';
  onSubmit() {
    this.errorMessage = '';

    if (!this.email) {
      this.errorMessage = 'Email không được để trống';
      return;
    }

    const gmailRegex = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;
    if (!gmailRegex.test(this.email)) {
      this.errorMessage = 'Vui lòng nhập email hợp lệ có đuôi @gmail.com';
      return;
    }

    this.userService.forgotPassword(this.email).subscribe({
      next: (response) => {
        console.log((response as any).message);
        this.toastrService.success((response as any).message);
      },
      error: (er) => {
        console.log(er);
        this.toastrService.error(er.error);
      },
    });
  }
}
