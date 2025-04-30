import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ItemService } from '../../core/services/item.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../core/services/user.service';
import { UtilitiesService } from '../../core/services/utilities.service';
import { CartService } from '../../core/services/cart.service';
import { Size } from '../../shared/models/size';
import { Item } from '../../shared/models/item';
import { Category } from '../../shared/models/category';
import { CartItem } from '../../shared/models/cart';
import { NzCardModule } from 'ng-zorro-antd/card';
import { CommonModule } from '@angular/common';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';

@Component({
  selector: 'app-item-modal',
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
  templateUrl: './item-modal.component.html',
  styleUrl: './item-modal.component.css',
})
export class ItemModalComponent {
  private itemService = inject(ItemService);
  private toastrService = inject(ToastrService);
  private userService = inject(UserService);
  private router = inject(Router);
  utilService = inject(UtilitiesService);
  cartService = inject(CartService);

  item?: Item; // selected item
  size?: Size; // selected size
  quantity: number = 1;
  categories: Category[] = [];

  isVisible = false;

  addToCart(): void {
    console.log('size', this.size);
    if (!this.userService.currentUser()) {
      this.router.navigate(['/dang-nhap']);
      this.toastrService.info('Vui lòng đăng nhập');
      return;
    }

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
      price: this.size.newPrice || this.size.price,
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

  showModal(id: number): void {
    this.size = undefined;
    this.quantity = 1;
    this.itemService.get(id).subscribe({
      next: (res) => {
        console.log(res);
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
}
