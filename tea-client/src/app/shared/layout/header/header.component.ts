import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { UserService } from '../../../core/services/user.service';
import { CommonModule } from '@angular/common';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { CartService } from '../../../core/services/cart.service';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { Discount } from '../../models/discount';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { CartItem } from '../../models/cart';
import {
  Order,
  OrderAddInStore,
  OrderAddOnline,
  OrderItemAdd,
} from '../../models/order';
import { OrderService } from '../../../core/services/order.service';
import { ToastrService } from 'ngx-toastr';
import { DiscountService } from '../../../core/services/discount.service';
import { paymentTypeList } from '../../../core/constants/payment';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { PaymentService } from '../../../core/services/payment.service';
import { PaymentResponse } from '../../models/payment';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    NzMenuModule,
    NzIconModule,
    RouterLink,
    CommonModule,
    ReactiveFormsModule,
    NzAvatarModule,
    NzDropDownModule,
    NzBadgeModule,
    NzDrawerModule,
    NzFormModule,
    FormsModule,
    NzInputModule,
    NzSelectModule,
    NzImageModule,
    NzToolTipModule,
    NzButtonModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  currentPage = 1;
  userService = inject(UserService);
  cartService = inject(CartService);
  utilService = inject(UtilitiesService);
  private orderService = inject(OrderService);
  private toastrService = inject(ToastrService);
  private discountService = inject(DiscountService);
  private paymentService = inject(PaymentService);
  private router = inject(Router);
  discount?: Discount;
  promotionCode: string = '';
  paymentTypeList = paymentTypeList;
  paymentType = this.paymentTypeList[0].value;
  phoneNumber: string = '';
  address: string = '';
  description: string = '';

  changePage(page: number) {
    this.currentPage = page;
  }

  visible = false;

  open(): void {
    this.visible = true;
  }

  close(): void {
    this.visible = false;
  }

  onPlus(item: CartItem) {
    this.cartService.addItemToCart(item);
  }

  onMinus(item: CartItem) {
    this.cartService.removeItemFromCart(item);
  }

  applyDiscount() {
    this.discountService.checkDiscount(this.promotionCode).subscribe({
      next: (res) => {
        this.discount = res as Discount;
      },
      error: (er) => {
        console.log('Không tìm thấy mã giảm giá');
      },
    });
  }

  createOrder() {
    if (this.paymentType === 'CreditCard') {
      this.goPayment();
      return;
    }
    const orderItemAddList: OrderItemAdd[] = [];
    this.cartService.cart()?.items.map((x) => {
      const orderItemAdd: OrderItemAdd = {
        itemName: x.itemName,
        price: x.price,
        itemSize: x.size,
        quantity: x.quantity,
        itemId: x.itemId,
        itemImg: x.itemImg,
      };
      orderItemAddList.push(orderItemAdd);
    });
    const orderAddOnline: OrderAddOnline = {
      userName: this.userService.currentUser()?.userName ?? '',
      paymentType: this.paymentType,
      items: orderItemAddList,
      discountId: this.discount?.id,
      discountPrice: this.calculateDiscountPrice(),
      customerName: this.userService.currentUser()?.fullName ?? '',
      customerAddress: this.address,
      customerPhone: this.phoneNumber,
      description: this.description,
    };
    this.orderService.addOrderOnline(orderAddOnline).subscribe({
      next: (res) => {
        const order = res as Order;
        this.toastrService.success('Tạo đơn thành công');
        this.cartService.deleteCart();
        this.discount = undefined;
        this.router.navigate(['/dat-hang-thanh-cong'], {
          queryParams: {
            id: order.id,
          },
        });
        this.close();
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  calculateDiscountPrice(): number | undefined {
    if (!this.discount) {
      return undefined;
    }

    const total = this.cartService.totals();

    if (this.discount.amountOff) {
      return this.discount.amountOff;
    }

    if (this.discount.percentOff) {
      return (total * this.discount.percentOff) / 100;
    }

    return undefined;
  }

  goPayment() {
    const paymentResponse: PaymentResponse = {
      username: this.userService.currentUser()?.userName || '',
      total: this.cartService.totals(),
      address: this.address,
      description: this.description,
      phoneNumber: this.phoneNumber,
    };

    if (this.discount) {
      paymentResponse.discountId = this.discount.id;
      paymentResponse.discountPrice = this.calculateDiscountPrice();
    }

    console.log(paymentResponse);

    this.paymentService.getPaymentUrl(paymentResponse).subscribe({
      next: (res) => {
        if (res) {
          localStorage.setItem('paymentProcessing', 'true');
          window.location.replace(res);
        }
      },
      error: (er) => {
        console.log(er);
      },
    });
  }
}
