import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { UserService } from '../../core/services/user.service';
import { CartService } from '../../core/services/cart.service';
import { UtilitiesService } from '../../core/services/utilities.service';
import { OrderService } from '../../core/services/order.service';
import { ToastrService } from 'ngx-toastr';
import { DiscountService } from '../../core/services/discount.service';
import { PaymentService } from '../../core/services/payment.service';
import { Discount } from '../../shared/models/discount';
import { paymentTypeList } from '../../core/constants/payment';
import { Order, OrderAddOnline, OrderItemAdd } from '../../shared/models/order';
import { PaymentResponse } from '../../shared/models/payment';
import { CartItem } from '../../shared/models/cart';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzDrawerModule,
    NzFormModule,
    FormsModule,
    NzInputModule,
    NzSelectModule,
    NzImageModule,
    NzToolTipModule,
    NzButtonModule,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  userService = inject(UserService);
  cartService = inject(CartService);
  utilService = inject(UtilitiesService);
  private orderService = inject(OrderService);
  private toastrService = inject(ToastrService);
  private discountService = inject(DiscountService);
  private paymentService = inject(PaymentService);
  private router = inject(Router);

  visible = false; // drawer

  discount?: Discount;
  promotionCode: string = '';
  paymentTypeList = paymentTypeList;
  paymentType = this.paymentTypeList[0].value;
  phoneNumber = computed(
    () => this.userService.currentUser()?.phoneNumber?.toString() || ''
  );
  address = computed(
    () => this.userService.currentUser()?.address.toString() || ''
  );

  phoneNumberInput = '';
  addressInput = '';

  description: string = '';

  updatePhoneNumber(event: Event) {
    const newValue = (event.target as HTMLInputElement).value;
    this.phoneNumberInput = newValue;
  }

  updateAddress(event: Event) {
    const newValue = (event.target as HTMLInputElement).value;
    this.addressInput = newValue;
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

    if (this.phoneNumberInput === '') {
      this.phoneNumberInput = this.phoneNumber();
    }

    if (this.addressInput === '') {
      this.addressInput = this.address();
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
      customerAddress: this.addressInput,
      customerPhone: this.phoneNumberInput,
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
      address: this.address(),
      description: this.description,
      phoneNumber: this.phoneNumber(),
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

  onPlus(item: CartItem) {
    this.cartService.addItemToCart(item);
  }

  onMinus(item: CartItem) {
    this.cartService.removeItemFromCart(item);
  }

  close(): void {
    this.visible = false;
  }
}
