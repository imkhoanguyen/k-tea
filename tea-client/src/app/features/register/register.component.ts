import { Component, inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { UserService } from '../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Register } from '../../shared/models/user';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDividerModule } from 'ng-zorro-antd/divider';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    CommonModule,
    NzCardModule,
    NzDividerModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private userService = inject(UserService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      userName: ['', Validators.required],
      fullName: ['', Validators.required],
      password: ['', Validators.required],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      address: ['', Validators.required],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    this.frm.controls['password'].valueChanges.subscribe({
      next: () => this.frm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }

  submitForm() {
    const userAdd: Register = {
      userName: this.frm.value.userName,
      fullName: this.frm.value.fullName,
      phoneNumber: this.frm.value.phoneNumber,
      email: this.frm.value.email,
      password: this.frm.value.password,
      address: this.frm.value.address,
    };

    this.userService.register(userAdd).subscribe({
      next: (res) => {
        this.toastrService.success('Đăng ký thành công');
        this.handleCancel();
      },
      error: (er) => {
        this.validationErrors = er;
        console.log(this.validationErrors);
        console.log(this.validationErrors?.length);
      },
    });
  }

  handleCancel() {
    this.frm.reset();
    this.validationErrors = [];
    this.router.navigateByUrl('/dang-nhap');
  }
}
