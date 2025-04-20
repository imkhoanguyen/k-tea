import { Component, inject } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { ActivatedRoute } from '@angular/router';
import { UtilitiesService } from '../../core/services/utilities.service';
import { Order } from '../../shared/models/order';
import { CommonModule, DatePipe } from '@angular/common';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

enum OrderStatus {
  PENDING = 'Pending',
  PROCESSING = 'Processing',
  COMPLETED = 'Completed',
  CANCELLED = 'Cancelled',
}

enum PaymentStatus {
  PENDING = 'Pending',
  PAID = 'Paid',
  CANCELLED = 'Cancelled',
}

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    NzSelectModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css',
})
export class OrderDetailComponent {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  utilService = inject(UtilitiesService);
  private toastrService = inject(ToastrService);
  id: number = 0;
  order?: Order;

  orderStatuses = Object.values(OrderStatus);
  paymentStatuses = Object.values(PaymentStatus);

  selectedOrderStatus?: string;
  selectedPaymentStatus?: string;

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params) => {
      const idParam = params.get('id');
      this.id = idParam ? +idParam : 0; // Chuyển string sang number, mặc định 0 nếu null
    });
    this.loadOrder();
  }

  loadOrder() {
    this.orderService.get(this.id).subscribe({
      next: (res) => {
        this.order = res as Order;
        this.selectedOrderStatus = this.order?.orderStatus;
        this.selectedPaymentStatus = this.order?.paymentStatus;
        console.log(this.selectedOrderStatus);
      },
      error: (er) => {
        console.log(er);
      },
    });
  }
  calTotal(order: Order) {
    const discountPrice = Number(order.discountPrice) || 0;
    let total =
      Number(order.subTotal) + Number(order.shippingFee) - discountPrice;
    if (total < 0) total = 0;
    return total;
  }

  updatePaymentStatus(newStatus: string) {
    if (this.order) {
      this.orderService
        .updatePaymenttatus(this.order?.id, newStatus)
        .subscribe({
          next: (_) => {
            this.toastrService.success(
              'Cập nhật trạng thái thanh toán thành công'
            );
          },
          error: (er) => {
            console.log(er);
          },
        });
    } else {
      this.toastrService.error('Có lỗi xảy ra vui lòng thử lại sau');
    }
  }

  updateOrderStatus(newStatus: string) {
    if (this.order) {
      this.orderService.updateOrderStatus(this.order?.id, newStatus).subscribe({
        next: (_) => {
          this.toastrService.success('Cập nhật trạng thái đơn hàng thành công');
        },
        error: (er) => {
          console.log(er);
        },
      });
    } else {
      this.toastrService.error('Có lỗi xảy ra vui lòng thử lại sau');
    }
  }
}
