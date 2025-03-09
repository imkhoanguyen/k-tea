import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { NzTableModule } from 'ng-zorro-antd/table';
import { CategoryService } from '../../../core/services/category.service';
import { Category, CategoryParams } from '../../../shared/models/category';
import { Pagination } from '../../../shared/models/base';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { CommonModule } from '@angular/common';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { FormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CategoryAddComponent } from '../category-add/category-add.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    NzTableModule,
    NzIconModule,
    NzButtonModule,
    CommonModule,
    NzPaginationModule,
    FormsModule,
    NzInputModule,
    CategoryAddComponent,
  ],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css',
})
export class CategoryListComponent implements OnInit {
  private categoryService = inject(CategoryService);
  categories?: Pagination<Category>;
  prm = new CategoryParams();
  @ViewChild(CategoryAddComponent) categoryAddComponent!: CategoryAddComponent;

  // helper add
  showAddModal() {
    this.categoryAddComponent.showModal();
  }

  handleEventAddParent(c: Category) {
    this.categories?.data.unshift(c);
  }

  // helper pagination
  onPageIndexChange(newPageNumber: number) {
    this.prm.pageIndex = newPageNumber;
    this.getPagination();
  }

  onPageSizeChange(newPageSize: number) {
    this.prm.pageSize = newPageSize;
    this.getPagination();
  }

  onSortChange(sortBy: string) {
    const currentSort = this.prm.orderBy;

    if (currentSort === sortBy) {
      this.prm.orderBy = `${sortBy}_desc`;
    } else if (currentSort === `${sortBy}_desc`) {
      this.prm.orderBy = sortBy;
    } else {
      this.prm.orderBy = sortBy;
    }
    this.getPagination();
  }

  onSearch() {
    this.getPagination();
  }

  // Helper table
  checked = false;
  loading = false;
  indeterminate = false;
  listOfCurrentPageData: readonly Category[] = [];
  setOfCheckedId = new Set<number>();

  ngOnInit(): void {
    this.getPagination();
  }

  getPagination() {
    this.categoryService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.categories = res;
        this.listOfCurrentPageData = res.data; // Cập nhật dữ liệu trang hiện tại
      },
      error: (er) => console.log(er),
    });
  }

  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  pdateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(value: boolean): void {
    this.listOfCurrentPageData.forEach((item) =>
      this.updateCheckedSet(item.id, value)
    );
    this.refreshCheckedStatus();
  }

  onCurrentPageDataChange($event: readonly Category[]): void {
    this.listOfCurrentPageData = $event;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.every((item) =>
      this.setOfCheckedId.has(item.id)
    );
    this.indeterminate =
      this.listOfCurrentPageData.some((item) =>
        this.setOfCheckedId.has(item.id)
      ) && !this.checked;
  }

  sendRequest(): void {
    console.log('Selected IDs:', Array.from(this.setOfCheckedId));
  }
}
