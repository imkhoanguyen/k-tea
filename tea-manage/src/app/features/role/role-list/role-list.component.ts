import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { RoleService } from '../../../core/services/role.service';
import { ToastrService } from 'ngx-toastr';
import { Pagination } from '../../../shared/models/base';
import { Role, RoleParams } from '../../../shared/models/role';
import { RoleAddComponent } from '../role-add/role-add.component';
import { RoleUpdateComponent } from '../role-update/role-update.component';

@Component({
  selector: 'app-role-list',
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
    RoleAddComponent,
    RoleUpdateComponent,
  ],
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.css',
})
export class RoleListComponent {
  private roleService = inject(RoleService);
  private modal = inject(NzModalService);
  private router = inject(Router);
  roles?: Pagination<Role>;
  prm = new RoleParams();
  @ViewChild(RoleAddComponent) roleAddComponent!: RoleAddComponent;
  @ViewChild(RoleUpdateComponent) roleUpdateComponent!: RoleUpdateComponent;

  showDeleteConfirm(id: string): void {
    this.modal.confirm({
      nzTitle: 'Bạn có chắc muốn xoá dòng này?',
      nzContent:
        '<b style="color: red;">Sau khi xoá sẽ không thể hoàn tác lại.</b>',
      nzOkText: 'Xác nhận',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.roleService.delete(id).subscribe({
          next: (_) => {
            if (this.roles) {
              const index = this.roles.data.findIndex((x) => x.id === id);
              if (index !== -1) {
                this.roles.data.splice(index, 1);
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

  ngOnInit(): void {
    this.getPagination();
  }

  getPagination() {
    this.roleService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.roles = res;
        console.log(res);
      },
      error: (er) => console.log(er),
    });
  }

  showAddModal() {
    this.roleAddComponent.showModal();
  }

  handleEventAddRole(role: Role) {
    this.roles?.data.unshift(role);
  }

  showUpdateModal(id: string) {
    this.roleUpdateComponent.id = id;
    this.roleUpdateComponent.showModal();
  }

  handleEventUpdateRole(role: Role) {
    if (this.roles) {
      const index = this.roles.data.findIndex((x) => x.id == role.id);
      if (index !== -1) {
        this.roles.data[index] = role;
      } else {
        this.getPagination();
      }
    } else {
      this.getPagination();
    }
  }

  goChangePermission(id: string) {
    this.router.navigate(['/cap-nhat-chuc-nang-cua-quyen', id]);
  }
}
