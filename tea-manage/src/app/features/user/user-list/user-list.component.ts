import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { UserService } from '../../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Pagination } from '../../../shared/models/base';
import { AppUser, UserParams } from '../../../shared/models/user';

@Component({
  selector: 'app-user-list',
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
    RouterLink,
    DatePipe,
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private modal = inject(NzModalService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  users?: Pagination<AppUser>;
  prm = new UserParams();

  goUpdatePage(username: string) {
    this.router.navigate(['/cap-nhat-nguoi-dung', username]);
  }

  showDeleteConfirm(username: string): void {
    // this.modal.confirm({
    //   nzTitle: 'Bạn có chắc muốn xoá dòng này?',
    //   nzContent:
    //     '<b style="color: red;">Sau khi xoá sẽ không thể hoàn tác lại.</b>',
    //   nzOkText: 'Xác nhận',
    //   nzOkType: 'primary',
    //   nzOkDanger: true,
    //   nzOnOk: () => {
    //     if (this.setOfCheckedId.size === 1) {
    //       const id = this.setOfCheckedId.values().next().value;
    //       if (id) {
    //         this.itemService.delete(id).subscribe({
    //           next: (_) => {
    //             // clear checked size items
    //             this.setOfCheckedId.clear();
    //             // reset checked items
    //             this.refreshCheckedStatus();
    //             // show message success
    //             this.toastrService.success('Xoá sản phẩm đã chọn thành công');
    //             // update items table
    //             if (this.items) {
    //               const index = this.items.data.findIndex((x) => x.id === id);
    //               if (index !== -1) {
    //                 this.items.data.splice(index, 1);
    //               } else {
    //                 this.getPagination();
    //               }
    //             } else {
    //               this.getPagination();
    //             }
    //           },
    //           error: (er) => {
    //             console.log(er);
    //           },
    //         });
    //       } else {
    //         this.toastrService.error('Vui lòng chọn lại sản phẩm cần xoá');
    //       }
    //       // remove list item
    //     } else if (this.setOfCheckedId.size > 1) {
    //       const listId = Array.from(this.setOfCheckedId);
    //       this.itemService.deletes(listId)?.subscribe({
    //         next: (_) => {
    //           // show message success
    //           this.toastrService.success(
    //             'Xoá những sản phẩm đã chọn thành công'
    //           );
    //           // clear checked size items
    //           this.setOfCheckedId.clear();
    //           // reset checked items
    //           this.refreshCheckedStatus();
    //           // update items table
    //           if (this.items) {
    //             this.items.data = this.items.data.filter(
    //               (x) => !listId.includes(x.id)
    //             );
    //           } else {
    //             this.getPagination();
    //           }
    //         },
    //       });
    //     } else {
    //       this.toastrService.error('Vui lòng chọn lại sản phẩm cần xoá');
    //     }
    //   },
    //   nzCancelText: 'Huỷ',
    //   nzOnCancel: () => console.log('Cancel'),
    // });
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

  ngOnInit(): void {
    this.getPagination();
  }

  getPagination() {
    this.userService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.users = res;
      },
      error: (er) => console.log(er),
    });
  }
}
