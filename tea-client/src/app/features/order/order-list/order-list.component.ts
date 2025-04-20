import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { Pagination } from '../../../shared/models/base';
import { OrderList, OrderParams } from '../../../shared/models/order';
import { OrderService } from '../../../core/services/order.service';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzSelectModule,
    NzInputModule,
    FormsModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzPaginationModule,
    NzDatePickerModule,
  ],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css',
})
export class OrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  utilService = inject(UtilitiesService);
  private toastrService = inject(ToastrService);
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  orders?: Pagination<OrderList>;
  prm = new OrderParams();
  footerRender = (): string => 'extra footer';
  dateRange: [Date, Date] | null = null;
  username: string = '';

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params) => {
      this.username = params.get('username') ?? '';
    });
    this.loadOrderList();
  }

  loadOrderList() {
    if (!this.username) {
      this.toastrService.info('Vui lòng đăng nhập');
      return;
    }
    this.prm.userName = this.username;
    if (this.dateRange) {
      this.prm.fromDate = this.dateRange[0]; // Ngày bắt đầu
      this.prm.toDate = this.dateRange[1]; // Ngày kết thúc
    } else {
      this.prm.fromDate = undefined;
      this.prm.toDate = undefined;
    }
    this.orderService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.orders = res;
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  onPageIndexChange(newPageNumber: number) {
    this.prm.pageIndex = newPageNumber;
    this.loadOrderList();
  }

  onPageSizeChange(newPageSize: number) {
    this.prm.pageSize = newPageSize;
    this.loadOrderList();
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
    this.loadOrderList();
  }

  refreshOrderList() {
    this.prm.fromDate = undefined;
    this.prm.search = '';
    this.prm.toDate = undefined;
    this.prm.maxAmount = undefined;
    this.prm.minAmount = undefined;
    this.dateRange = null;
    this.loadOrderList();
  }

  goDetail(id: number) {
    this.router.navigate(['/chi-tiet-don-hang', id]);
  }
}
