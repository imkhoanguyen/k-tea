import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { CommonModule, DatePipe } from '@angular/common';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    DatePipe,
    NzAlertModule,
    NzButtonModule,
    RouterLink,
  ],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css',
})
export class OrderDetailComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  utilService = inject(UtilitiesService);
  id: number = 0;
  order?: Order;
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
}
