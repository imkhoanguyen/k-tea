import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { DiscountService } from '../../../core/services/discount.service';
import { ToastrService } from 'ngx-toastr';
import { Discount, DiscountParams } from '../../../shared/models/discount';
import { Pagination } from '../../../shared/models/base';
import { DiscountAddComponent } from '../discount-add/discount-add.component';
import { DiscountUpdateComponent } from '../discount-update/discount-update.component';
import { UtilitiesService } from '../../../core/services/utilities.service';

@Component({
  selector: 'app-discount-list',
  standalone: true,
  imports: [
    NzTableModule,
    NzIconModule,
    NzButtonModule,
    CommonModule,
    NzPaginationModule,
    FormsModule,
    NzInputModule,
    NzModalModule,
    DiscountAddComponent,
    DiscountUpdateComponent,
  ],
  templateUrl: './discount-list.component.html',
  styleUrl: './discount-list.component.css',
})
export class DiscountListComponent implements OnInit {
  private discountService = inject(DiscountService);
  private modal = inject(NzModalService);
  private toastrService = inject(ToastrService);
  utilService = inject(UtilitiesService);
  discounts?: Pagination<Discount>;
  prm = new DiscountParams();
  @ViewChild(DiscountAddComponent) discountAddComponent!: DiscountAddComponent;
  @ViewChild(DiscountUpdateComponent)
  discountUpdateComponent!: DiscountUpdateComponent;

  // helper add
  showAddModal() {
    this.discountAddComponent.showModal();
  }

  handleEventAdd(c: Discount) {
    this.discounts?.data.unshift(c);
  }

  // update
  showUpdateModal() {
    if (this.setOfCheckedId.size === 1) {
      const id = this.setOfCheckedId.values().next().value; // Lấy giá trị đầu tiên
      this.discountUpdateComponent.id = id ?? 0;
      this.discountUpdateComponent.showModal();
    }
  }

  handleEventUpdate(c: Discount) {
    if (this.discounts) {
      const index = this.discounts.data.findIndex((x) => x.id == c.id);
      if (index !== -1) {
        this.discounts.data[index] = c;
      }
    }
  }

  //delete
  showDeleteConfirm(): void {
    this.modal.confirm({
      nzTitle: 'Bạn có chắc muốn xoá dòng này?',
      nzContent:
        '<b style="color: red;">Sau khi xoá sẽ không thể hoàn tác lại.</b>',
      nzOkText: 'Xác nhận',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        if (this.setOfCheckedId.size === 1) {
          const id = this.setOfCheckedId.values().next().value;
          if (id) {
            this.discountService.delete(id).subscribe({
              next: (_) => {
                // show success message
                this.toastrService.success(
                  'Xoá mã giảm giá đã chọn thành công'
                );

                // update checked status
                this.refreshCheckedStatus();
                this.setOfCheckedId.clear();

                // update discounts table
                if (this.discounts) {
                  const index = this.discounts.data.findIndex(
                    (x) => x.id === id
                  );
                  if (index !== -1) {
                    this.discounts.data.splice(index, 1);
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
            this.toastrService.error('Vui lòng chọn lại mã giảm giá cần xoá');
          }
        } else if (this.setOfCheckedId.size > 1) {
          const listId = Array.from(this.setOfCheckedId);
          this.discountService.deletes(listId)?.subscribe({
            next: (_) => {
              // show success message
              this.toastrService.success(
                'Xoá những mã giảm giá đã chọn thành công'
              );

              // update checked status
              this.refreshCheckedStatus();
              this.setOfCheckedId.clear();

              // update discounts table
              if (this.discounts) {
                this.discounts.data = this.discounts.data.filter(
                  (x) => !listId.includes(x.id)
                );
              } else {
                this.getPagination();
              }
            },
          });
        } else {
          this.toastrService.error('Vui lòng chọn lại mã giảm giá cần xoá');
        }
      },
      nzCancelText: 'Huỷ',
      nzOnCancel: () => console.log('Cancel'),
    });
  }

  // pagination
  onPageIndexChange(newPageNumber: number) {
    this.prm.pageIndex = newPageNumber;
    this.getPagination();
    this.refreshCheckedStatus();
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
  setOfCheckedId = new Set<number>();

  ngOnInit(): void {
    this.getPagination();
  }

  getPagination() {
    this.discountService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.discounts = res;
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

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(value: boolean): void {
    if (this.discounts) {
      this.discounts.data.forEach((discount) =>
        this.updateCheckedSet(discount.id, value)
      );
      this.refreshCheckedStatus();
    }
  }

  refreshCheckedStatus(): void {
    if (this.discounts) {
      this.checked = this.discounts.data.every((discount) =>
        this.setOfCheckedId.has(discount.id)
      );
      this.indeterminate =
        this.discounts.data.some((discount) =>
          this.setOfCheckedId.has(discount.id)
        ) && !this.checked;
    }
  }
}
