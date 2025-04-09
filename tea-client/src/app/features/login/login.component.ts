import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFormModule } from 'ng-zorro-antd/form';
import { UserService } from '../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzCardModule,
    CommonModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  private toastrSevice = inject(ToastrService);

  ngOnInit(): void {
    this.initForm();
  }

  ngOnDestroy(): void {
    this.validationErrors = [];
    this.frm.reset();
  }

  initForm() {
    this.frm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submitForm(): void {
    const userName = this.frm.value.username;
    const password = this.frm.value.password;
    this.userService.login(userName, password).subscribe({
      next: (_) => {
        this.toastrSevice.success('Đăng nhập thành công');
        this.router.navigate(['/']);
      },
      error: (er) => {
        console.log(er);
        this.validationErrors = er;
      },
    });
  }
}
