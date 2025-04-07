import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { DiscountService } from '../../../core/services/discount.service';
import { ToastrService } from 'ngx-toastr';
import { Discount, DiscountAdd } from '../../../shared/models/discount';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CommonModule } from '@angular/common';
import { NzSelectModule } from 'ng-zorro-antd/select';

@Component({
  selector: 'app-discount-add',
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
  templateUrl: './discount-add.component.html',
  styleUrl: './discount-add.component.css',
})
export class DiscountAddComponent implements OnInit {
  private discountService = inject(DiscountService);
  private toastrService = inject(ToastrService);
  @Output() discountAdded = new EventEmitter<Discount>();
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
      promotionCode: ['', Validators.required],
      discount: [0, Validators.required],
      discountType: ['amoutOff', Validators.required],
    });
  }

  submitForm() {
    const discountAdd: DiscountAdd = {
      name: this.frm.value.name,
      promotionCode: this.frm.value.promotionCode,
    };

    const discount = this.frm.value.discountType;
    if (discount === 'amoutOff') {
      discountAdd.amountOff = this.frm.value.discount;
    }
    if (discount === 'percentOff') {
      discountAdd.percentOff = this.frm.value.discount;
    }
    this.discountService.add(discountAdd).subscribe({
      next: (res) => {
        this.toastrService.success('Thêm mã giảm giá thành công');
        this.discountAdded.emit(res as Discount);
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
    this.frm.patchValue({
      discountType: 'amoutOff',
    });
    this.validationErrors = [];
  }

  showModal(): void {
    this.isVisible = true;
  }
}
