import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
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
import { OrderAddInStore, OrderItemAdd } from '../../models/order';
import { OrderService } from '../../../core/services/order.service';
import { ToastrService } from 'ngx-toastr';
import { DiscountService } from '../../../core/services/discount.service';
import { paymentTypeList } from '../../../core/constants/payment';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzButtonModule } from 'ng-zorro-antd/button';

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
  discount?: Discount;
  promotionCode: string = '';
  paymentTypeList = paymentTypeList;
  paymentType = this.paymentTypeList[0].value;

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
    const orderAddInStore: OrderAddInStore = {
      createdById: this.userService.currentUser()?.userName ?? '',
      paymentType: this.paymentType,
      items: orderItemAddList,
      discountId: this.discount?.id,
      discountPrice: this.calculateDiscountPrice(),
    };
    this.orderService.addOrderInStore(orderAddInStore).subscribe({
      next: (res) => {
        this.toastrService.success('Tạo đơn thành công');
        this.cartService.deleteCart();
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
      return total - this.discount.amountOff;
    }

    if (this.discount.percentOff) {
      return total - (total * this.discount.percentOff) / 100;
    }

    return undefined;
  }
}
