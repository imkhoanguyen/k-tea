import { Component, inject, OnInit } from '@angular/core';
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
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { ItemService } from '../../core/services/item.service';
import { UtilitiesService } from '../../core/services/utilities.service';
import { CartService } from '../../core/services/cart.service';
import { Pagination } from '../../shared/models/base';
import { Item, ItemParams } from '../../shared/models/item';
import { Size } from '../../shared/models/size';
import { Discount } from '../../shared/models/discount';
import { paymentTypeList } from '../../core/constants/payment';
import { ToastrService } from 'ngx-toastr';
import { CartItem } from '../../shared/models/cart';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../shared/models/category';
import { UserService } from '../../core/services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-shop',
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
    NzPaginationModule,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.css',
})
export class ShopComponent implements OnInit {
  private itemService = inject(ItemService);
  private toastrService = inject(ToastrService);
  private categoryService = inject(CategoryService);
  private userService = inject(UserService);
  private router = inject(Router);
  utilService = inject(UtilitiesService);
  cartService = inject(CartService);
  items?: Pagination<Item>;
  prm = new ItemParams();
  item?: Item; // selected item
  size?: Size; // selected size
  quantity: number = 1;
  categories: Category[] = [];

  ngOnInit(): void {
    this.getPagination();
    this.getAllCategory();
  }

  getPagination() {
    this.itemService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.items = res;
      },
      error: (er) => console.log(er),
    });
  }

  getAllCategory() {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        this.categories = res;
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  isVisible = false;

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

  // pagination
  onPageIndexChange(newPageNumber: number) {
    this.prm.pageIndex = newPageNumber;
    this.getPagination();
  }

  onPageSizeChange(newPageSize: number) {
    this.prm.pageSize = newPageSize;
    this.getPagination();
  }

  onSearch() {
    this.getPagination();
  }

  selectCategory(categoryId: number) {
    this.prm.categoryId = categoryId;

    this.getPagination();
  }
}
