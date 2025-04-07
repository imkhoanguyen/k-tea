import { Component, inject, OnInit } from '@angular/core';
import { NzCardModule } from 'ng-zorro-antd/card';
import { ItemService } from '../../../core/services/item.service';
import { Pagination } from '../../../shared/models/base';
import { Item, ItemParams } from '../../../shared/models/item';
import { CommonModule } from '@angular/common';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { ToastrService } from 'ngx-toastr';
import { Size } from '../../../shared/models/size';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CartService } from '../../../core/services/cart.service';
import { Cart, CartItem } from '../../../shared/models/cart';
import { UserService } from '../../../core/services/user.service';
import { Discount } from '../../../shared/models/discount';
import { DiscountService } from '../../../core/services/discount.service';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { paymentTypeList } from '../../../constants/payment';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { OrderService } from '../../../core/services/order.service';
import { OrderAddInStore, OrderItemAdd } from '../../../shared/models/order';

@Component({
  selector: 'app-sale-list',
  standalone: true,
  imports: [
    NzCardModule,
    CommonModule,
    NzImageModule,
    NzButtonModule,
    NzToolTipModule,
    NzModalModule,
    NzRadioModule,
    FormsModule,
    ReactiveFormsModule,
    NzInputModule,
    NzSelectModule,
  ],
  templateUrl: './sale-list.component.html',
  styleUrl: './sale-list.component.css',
})
export class SaleListComponent implements OnInit {
  private itemService = inject(ItemService);
  private toastrService = inject(ToastrService);
  private userService = inject(UserService);
  private discountService = inject(DiscountService);
  private orderService = inject(OrderService);
  utilService = inject(UtilitiesService);
  cartService = inject(CartService);
  items?: Pagination<Item>;
  prm = new ItemParams();
  item?: Item; // selected item
  size?: Size; // selected size
  quantity: number = 1;
  discount?: Discount;
  promotionCode: string = '';
  paymentTypeList = paymentTypeList;
  paymentType = this.paymentTypeList[0].value;

  ngOnInit(): void {
    this.getPagination();
    this.cartService
      .getCart(this.userService.currentUser()?.userName || '')
      .subscribe({
        next: (cart) => {
          // Cart đã được load và lưu trong cartService.cart()
          console.log('Cart loaded', cart);
        },
        error: (err) => {
          console.error('Failed to load cart', err);
          this.toastrService.error('Không thể tải giỏ hàng');
        },
      });
    console.log(this.paymentTypeList);
  }

  getPagination() {
    this.itemService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.items = res;
      },
      error: (er) => console.log(er),
    });
  }

  isVisible = false;

  showModal(id: number): void {
    this.itemService.get(id).subscribe({
      next: (res) => {
        this.item = res as Item;
        this.isVisible = true;
      },
      error: (er) => {
        this.toastrService.error(
          'Xảy ra lỗi khi tải sản phẩm. Vui lòng chọn lại'
        );
        console.log(er);
      },
    });
  }

  addToCart(): void {
    if (!this.size) {
      this.toastrService.info('Vui lòng chọn loại sản phẩm');
      return;
    }
    if (!this.item) {
      this.toastrService.info('Vui lòng chọn lại sản phẩm');
      return;
    }
    var cartItem: CartItem = {
      itemName: this.item.name,
      size: this.size.name,
      itemImg: this.item.imgUrl,
      itemId: this.item.id,
      quantity: this.quantity,
      price: this.size.price,
    };
    this.cartService
      .addItemToCart(cartItem, cartItem.quantity)
      .then((success) => {
        if (success) {
          this.toastrService.success('Thêm sản phẩm vào giỏ hàng thành công');
          this.isVisible = false;
        } else {
          this.toastrService.error('Thêm sản phẩm vào giỏ hàng thất bại');
        }
      });
  }

  handleCancel(): void {
    this.isVisible = false;
    this.item = undefined;
    this.size = undefined;
    this.quantity = 1;
  }

  onSizeChange(size: Size) {
    this.size = size;
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
