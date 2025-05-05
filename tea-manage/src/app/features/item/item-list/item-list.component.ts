import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { ToastrService } from 'ngx-toastr';
import { ImportResult, Item, ItemParams } from '../../../shared/models/item';
import { Pagination } from '../../../shared/models/base';
import { ItemService } from '../../../core/services/item.service';
import { NzImageModule } from 'ng-zorro-antd/image';
import { Router, RouterLink } from '@angular/router';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { NzUploadFile, NzUploadModule } from 'ng-zorro-antd/upload';

@Component({
  selector: 'app-item-list',
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
    NzImageModule,
    RouterLink,
    NzDropDownModule,
    NzUploadModule,
  ],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css',
})
export class ItemListComponent {
  private itemService = inject(ItemService);
  private modal = inject(NzModalService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  private utilService = inject(UtilitiesService);
  items?: Pagination<Item>;
  prm = new ItemParams();

  goUpdatePage() {
    if (this.setOfCheckedId.size === 1) {
      const id = this.setOfCheckedId.values().next().value; // Lấy giá trị đầu tiên
      if (id) {
        this.router.navigate(['/cap-nhat-san-pham', id]);
      } else {
        this.toastrService.error('Vui lòng chọn lại sản phẩm cần cập nhật');
      }
    }
  }

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
            this.itemService.delete(id).subscribe({
              next: (_) => {
                // clear checked size items
                this.setOfCheckedId.clear();

                // reset checked items
                this.refreshCheckedStatus();

                // show message success
                this.toastrService.success('Xoá sản phẩm đã chọn thành công');

                // update items table
                if (this.items) {
                  const index = this.items.data.findIndex((x) => x.id === id);
                  if (index !== -1) {
                    this.items.data.splice(index, 1);
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
            this.toastrService.error('Vui lòng chọn lại sản phẩm cần xoá');
          }
          // remove list item
        } else if (this.setOfCheckedId.size > 1) {
          const listId = Array.from(this.setOfCheckedId);
          this.itemService.deletes(listId)?.subscribe({
            next: (_) => {
              // show message success
              this.toastrService.success(
                'Xoá những sản phẩm đã chọn thành công'
              );

              // clear checked size items
              this.setOfCheckedId.clear();

              // reset checked items
              this.refreshCheckedStatus();

              // update items table
              if (this.items) {
                this.items.data = this.items.data.filter(
                  (x) => !listId.includes(x.id)
                );
              } else {
                this.getPagination();
              }
            },
          });
        } else {
          this.toastrService.error('Vui lòng chọn lại sản phẩm cần xoá');
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
  }

  onPageSizeChange(newPageSize: number) {
    this.prm.pageSize = newPageSize;
    this.getPagination();
    this.refreshCheckedStatus();
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
    this.itemService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.items = res;
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
    if (this.items) {
      this.items.data.forEach((item) => this.updateCheckedSet(item.id, value));
      this.refreshCheckedStatus();
    }
  }

  refreshCheckedStatus(): void {
    if (this.items) {
      this.checked = this.items.data.every((item) =>
        this.setOfCheckedId.has(item.id)
      );
      this.indeterminate =
        this.items.data.some((item) => this.setOfCheckedId.has(item.id)) &&
        !this.checked;
    }
  }

  exportTemplateUpdate() {
    const listId = Array.from(this.setOfCheckedId);
    this.itemService.exportTemplateUpdate(listId).subscribe({
      next: (res) => {
        this.utilService.downloadFile(res, `template_update_item.xlsx`);
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  fileList: NzUploadFile[] = [];
  importResult?: ImportResult;

  // Hàm xử lý trước khi upload
  beforeUploadUpdate = (file: NzUploadFile): boolean => {
    this.handleUpdateImport(file as unknown as File);
    return false; // Return false để ngăn tự động upload
  };

  // Hàm xử lý trước khi upload
  beforeUploadAdd = (file: NzUploadFile): boolean => {
    this.handleAddImport(file as unknown as File);
    return false; // Return false để ngăn tự động upload
  };

  // Hàm xử lý import
  handleUpdateImport(file: File): void {
    if (!file) {
      this.toastrService.info('Please select a file first');
      return;
    }

    // Kiểm tra định dạng file
    if (!file.name.endsWith('.xlsx')) {
      this.toastrService.error('Only .xlsx files are allowed');
      return;
    }

    this.itemService.importUpdateItem(file).subscribe({
      next: (result) => {
        this.importResult = result;
        this.showModal();
        this.fileList = []; // Reset file list sau khi import thành công
      },
      error: (err) => {
        this.toastrService.error(
          err.error?.errors?.join('\n') || 'An error occurred during import'
        );
        this.fileList = []; // Reset file list khi có lỗi
      },
    });
  }

  // Hàm xử lý import
  handleAddImport(file: File): void {
    if (!file) {
      this.toastrService.info('Please select a file first');
      return;
    }

    // Kiểm tra định dạng file
    if (!file.name.endsWith('.xlsx')) {
      this.toastrService.error('Only .xlsx files are allowed');
      return;
    }

    this.itemService.importAddItem(file).subscribe({
      next: (result) => {
        this.importResult = result;
        this.showModal();
        this.fileList = []; // Reset file list sau khi import thành công
      },
      error: (err) => {
        this.toastrService.error(
          err.error?.errors?.join('\n') || 'An error occurred during import'
        );
        this.fileList = []; // Reset file list khi có lỗi
      },
    });
  }

  isVisible = false;

  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {
    console.log('Button ok clicked!');
    this.isVisible = false;
  }

  handleCancel(): void {
    console.log('Button cancel clicked!');
    this.isVisible = false;
  }

  exportTemplateAdd() {
    this.itemService.exportTemplateAdd().subscribe({
      next: (res) => {
        this.utilService.downloadFile(res, `template_add_item.xlsx`);
      },
      error: (er) => {
        console.log(er);
      },
    });
  }
}
