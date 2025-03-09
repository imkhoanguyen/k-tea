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
import { CategoryUpdateComponent } from '../category-update/category-update.component';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { ToastrService } from 'ngx-toastr';

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
    CategoryUpdateComponent,
    NzModalModule,
  ],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css',
})
export class CategoryListComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private modal = inject(NzModalService);
  private toastrService = inject(ToastrService);
  categories?: Pagination<Category>;
  prm = new CategoryParams();
  @ViewChild(CategoryAddComponent) categoryAddComponent!: CategoryAddComponent;
  @ViewChild(CategoryUpdateComponent)
  categoryUpdateComponent!: CategoryUpdateComponent;

  // helper add
  showAddModal() {
    this.categoryAddComponent.showModal();
  }

  handleEventAddParent(c: Category) {
    this.categories?.data.unshift(c);
  }

  // update
  showUpdateModal() {
    if (this.setOfCheckedId.size === 1) {
      const id = this.setOfCheckedId.values().next().value; // Lấy giá trị đầu tiên
      this.categoryUpdateComponent.id = id ?? 0;
      this.categoryUpdateComponent.showModal();
    }
  }

  handleEventUpdate(c: Category) {
    if (this.categories) {
      const index = this.categories.data.findIndex((x) => x.id == c.id);
      if (index !== -1) {
        this.categories.data[index] = c;
      }
    }
  }

  //delete
  showDeleteConfirm(): void {
    this.modal.confirm({
      nzTitle: 'Bạn có chắc muốn xoá dòng này?',
      nzContent:
        '<b style="color: red;">Sau khi xoá sẽ không thể hoàn tác lại.</b>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        if (this.setOfCheckedId.size === 1) {
          const id = this.setOfCheckedId.values().next().value;
          if (id) {
            this.categoryService.delete(id).subscribe({
              next: (_) => {
                this.toastrService.success('Xoá danh mục đã chọn thành công');
                if (this.categories) {
                  const index = this.categories.data.findIndex(
                    (x) => x.id === id
                  );
                  if (index !== -1) {
                    this.categories.data.splice(index, 1);
                  } else {
                    this.getPagination();
                  }
                } else {
                  this.getPagination();
                }
              },
              error: (er) => {
                console.log(er);
              },
            });
          } else {
            this.toastrService.error('Vui lòng chọn lại danh mục cần xoá');
          }
        } else if (this.setOfCheckedId.size > 1) {
          const listId = Array.from(this.setOfCheckedId);
          this.categoryService.deletes(listId)?.subscribe({
            next: (_) => {
              this.toastrService.success(
                'Xoá những danh mục đã chọn thành công'
              );
              if (this.categories) {
                this.categories.data = this.categories.data.filter(
                  (x) => !listId.includes(x.id)
                );
              } else {
                this.getPagination();
              }
            },
          });
        }
      },
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel'),
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

  // table
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
