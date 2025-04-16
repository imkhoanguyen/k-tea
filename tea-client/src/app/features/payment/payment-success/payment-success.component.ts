import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { PaymentService } from '../../../core/services/payment.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/models/order';
import { CartService } from '../../../core/services/cart.service';
import { CommonModule } from '@angular/common';
import { UtilitiesService } from '../../../core/services/utilities.service';

@Component({
  selector: 'app-payment-success',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payment-success.component.html',
  styleUrl: './payment-success.component.css',
})
export class PaymentSuccessComponent implements OnInit, OnDestroy {
  private paymentService = inject(PaymentService);
  private activatedRoute = inject(ActivatedRoute);
  private orderService = inject(OrderService);
  private cartService = inject(CartService);
  utilService = inject(UtilitiesService);
  order?: Order;

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      const id = +params['id'] || 0;
      this.orderService.get(id).subscribe({
        next: (res) => {
          this.order = res as Order;
          console.log(this.order);
        },
        error: (er) => {
          console.log(er);
        },
      });
    });
    this.cartService.deleteCart();
  }

  ngOnDestroy(): void {
    this.order = undefined;
    this.paymentService.paymentSuccessed = false;
  }

  calFinal() {
    if (this.order) {
      const discountPrice = Number(this.order.discountPrice) || 0;

      if (this.calTotal() - discountPrice < 0) return 0;
      return this.calTotal() - discountPrice;
    }
    return 0;
  }

  calTotal() {
    if (this.order) {
      const subTotal = Number(this.order.subTotal) || 0;
      const shippingFee = Number(this.order.shippingFee) || 0;

      return subTotal + shippingFee;
    }
    return 0;
  }
}
