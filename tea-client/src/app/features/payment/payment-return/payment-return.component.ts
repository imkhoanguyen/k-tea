import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { PaymentService } from '../../../core/services/payment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentRequest } from '../../../shared/models/payment';
import { NgxSpinnerComponent, NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-payment-return',
  standalone: true,
  imports: [NgxSpinnerComponent],
  templateUrl: './payment-return.component.html',
  styleUrl: './payment-return.component.css',
})
export class PaymentReturnComponent implements OnInit, OnDestroy {
  private paymentService = inject(PaymentService);
  private activateRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private spinner = inject(NgxSpinnerService);

  ngOnInit(): void {
    this.spinner.show();
    const queryParams = this.activateRoute.snapshot.queryParams;

    // Parse JSON từ vnp_OrderInfo
    let orderInfo;
    try {
      orderInfo = JSON.parse(queryParams['vnp_OrderInfo']); // Giải mã JSON
    } catch (e) {
      console.error('Failed to parse vnp_OrderInfo', e);
      orderInfo = {};
    }

    const paymentRequest: PaymentRequest = {
      responseCode: queryParams['vnp_ResponseCode'],
      username: orderInfo.UserName,
      discountId: +orderInfo.DiscountId || 0,
      discountPrice: +orderInfo.DiscountPrice || 0,
      address: orderInfo.Address,
      phoneNumber: orderInfo.PhoneNumber,
      description: orderInfo.Description,
    };

    console.log('Payment Request:', paymentRequest);
    this.paymentService.getPaymentReturn(paymentRequest).subscribe({
      next: (res) => {
        if (res != null) {
          this.paymentService.paymentSuccessed = true;
          this.router.navigate(['/dat-hang-thanh-cong'], {
            queryParams: {
              id: res.id,
            },
          });
        } else {
          this.paymentService.paymentFailed = true;
          this.router.navigate(['/thanh-toan-that-bai']);
        }
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  ngOnDestroy(): void {
    this.spinner.hide();
    localStorage.removeItem('paymentProcessing');
  }
}
