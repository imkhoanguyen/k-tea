import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
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
import { NzSelectModule } from 'ng-zorro-antd/select';
import { DiscountService } from '../../../core/services/discount.service';
import { ToastrService } from 'ngx-toastr';
import { Discount, DiscountUpdate } from '../../../shared/models/discount';

@Component({
  selector: 'app-discount-update',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
    FormsModule,
    NzSelectModule,
  ],
  templateUrl: './discount-update.component.html',
  styleUrl: './discount-update.component.css',
})
export class DiscountUpdateComponent implements OnInit {
  private discountService = inject(DiscountService);
  private toastrService = inject(ToastrService);
  @Output() discountUpdated = new EventEmitter<Discount>();
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  isVisible = false;
  id: number = 0;

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      name: ['', Validators.required],
      promotionCode: ['', Validators.required],
      discount: [0, Validators.required],
      discountType: ['', Validators.required],
    });
  }

  pathValue(d: Discount) {
    this.id = d.id;
    this.frm.patchValue({
      name: d.name,
      promotionCode: d.promotionCode,
    });

    if (d.amountOff) {
      this.frm.patchValue({
        discount: d.amountOff,
        discountType: 'amoutOff',
      });
    }

    if (d.percentOff) {
      this.frm.patchValue({
        discount: d.percentOff,
        discountType: 'percentOff',
      });
    }
  }

  showModal(): void {
    if (this.id !== 0) this.loadDiscount();
    else this.toastrService.error('Vui lòng chọn lại mã giảm giá cần sửa');
  }

  loadDiscount() {
    this.discountService.get(this.id).subscribe({
      next: (res) => {
        this.isVisible = true;
        this.pathValue(res as Discount);
      },
      error: (er) => {
        console.log(er);
        this.handleCancel();
      },
    });
  }

  submitForm() {
    const discountUpdate: DiscountUpdate = {
      id: this.id,
      name: this.frm.value.name,
      promotionCode: this.frm.value.promotionCode,
    };
    const discount = this.frm.value.discountType;
    if (discount === 'amoutOff') {
      discountUpdate.amountOff = this.frm.value.discount;
    }
    if (discount === 'percentOff') {
      discountUpdate.percentOff = this.frm.value.discount;
    }
    this.discountService.update(discountUpdate).subscribe({
      next: (res) => {
        this.toastrService.success('Cập nhật mã giảm giá thành công');
        this.discountUpdated.emit(res as Discount);
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
}
