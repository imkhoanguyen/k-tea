import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { NzCardModule } from 'ng-zorro-antd/card';
import { CommonModule } from '@angular/common';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { ItemService } from '../../core/services/item.service';
import { UtilitiesService } from '../../core/services/utilities.service';
import { CartService } from '../../core/services/cart.service';
import { Pagination } from '../../shared/models/base';
import { Item, ItemParams } from '../../shared/models/item';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../shared/models/category';
import { ItemModalComponent } from '../item-modal/item-modal.component';

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
    NzPaginationModule,
    ItemModalComponent,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.css',
})
export class ShopComponent implements OnInit {
  private itemService = inject(ItemService);
  private categoryService = inject(CategoryService);
  utilService = inject(UtilitiesService);
  cartService = inject(CartService);
  items?: Pagination<Item>;
  prm = new ItemParams();
  categories: Category[] = [];
  @ViewChild(ItemModalComponent) itemComponent!: ItemModalComponent;

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

  showModal(id: number): void {
    this.itemComponent.showModal(id);
  }
}
