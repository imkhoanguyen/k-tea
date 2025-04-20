import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import * as echarts from 'echarts/core';
import { BarChart, LineChart } from 'echarts/charts';
import {
  DatasetComponent,
  GridComponent,
  LegendComponent,
  TitleComponent,
  TooltipComponent,
} from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { ReportService } from '../../core/services/report.service';
import {
  DailyRevenueInMonth,
  Report,
  TopSellingItem,
} from '../../shared/models/report';
import { UtilitiesService } from '../../core/services/utilities.service';
echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  DatasetComponent,
  CanvasRenderer,
  LineChart,
  BarChart,
  LegendComponent,
]);

import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { OrderList, OrderParams } from '../../shared/models/order';
import { OrderService } from '../../core/services/order.service';
import { Pagination } from '../../shared/models/base';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [
    CommonModule,
    NzStatisticModule,
    NzCardModule,
    NzIconModule,
    NzTableModule,
    NzImageModule,
    NzToolTipModule,
    NgxEchartsDirective,
    NzSelectModule,
    NzInputModule,
    FormsModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzPaginationModule,
    NzDatePickerModule,
    DatePipe,
  ],
  templateUrl: './report.component.html',
  styleUrl: './report.component.css',
  providers: [provideEchartsCore({ echarts })],
})
export class ReportComponent implements OnInit, OnDestroy {
  private reportService = inject(ReportService);
  private orderService = inject(OrderService);
  utilService = inject(UtilitiesService);
  private router = inject(Router);

  topCountItem = 5;
  month: number;
  year: number;
  report?: Report;
  dailyRevenueInMonth: DailyRevenueInMonth[] = [];
  topSellingItem: TopSellingItem[] = [];
  orders?: Pagination<OrderList>;
  prm = new OrderParams();
  footerRender = (): string => 'extra footer';
  dateRange: [Date, Date] | null = null;

  // Chart options
  chartOptions?: EChartsCoreOption;
  private chartTimer: any;

  constructor() {
    const currentDate = new Date();
    this.month = currentDate.getMonth() + 1;
    this.year = currentDate.getFullYear();
  }

  ngOnInit(): void {
    this.loadReport();
    this.loadDailyRevenueInMonth();
    this.loadTopSellingItems();
    this.initChart();
    this.loadOrderList();
  }

  ngOnDestroy(): void {
    if (this.chartTimer) {
      clearInterval(this.chartTimer);
    }
  }

  loadReport() {
    this.reportService.getReport().subscribe({
      next: (res) => {
        this.report = res as Report;
      },
      error: (err) => {
        console.error('Error loading report:', err);
      },
    });
  }

  loadTopSellingItems() {
    this.reportService.getTopSellingItem(this.topCountItem).subscribe({
      next: (res) => {
        this.topSellingItem = res;
      },
      error: (err) => {
        console.error('Error loading top selling items:', err);
      },
    });
  }

  loadDailyRevenueInMonth() {
    this.reportService.getDailyRevenueInMonth(this.month, this.year).subscribe({
      next: (res) => {
        this.dailyRevenueInMonth = res;
        this.updateChartWithRealData();
      },
      error: (err) => {
        console.error('Error loading daily revenue:', err);
      },
    });
  }

  loadOrderList() {
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

  private initChart() {
    this.chartOptions = {
      title: {
        text: 'Doanh thu theo ngày',
        left: 'center',
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow',
        },
        formatter: (params: any) => {
          const data = params[0].data;
          return `Ngày ${data[0]}: ${this.utilService.formatVND(data[1])}`;
        },
      },
      xAxis: {
        type: 'category',
        data: [],
        axisLabel: {
          rotate: 45,
        },
      },
      yAxis: {
        type: 'value',
        name: 'Doanh thu (VND)',
        axisLabel: {
          formatter: (value: number) => {
            return this.utilService.formatVND(value);
          },
        },
      },
      series: [
        {
          name: 'Doanh thu',
          type: 'bar',
          data: [],
          itemStyle: {
            color: '#1890ff',
          },
        },
      ],
      grid: {
        containLabel: true,
        left: '3%',
        right: '4%',
        bottom: '3%',
        top: '15%',
      },
    };
  }

  private updateChartWithRealData() {
    if (!this.dailyRevenueInMonth || this.dailyRevenueInMonth.length === 0)
      return;

    const days = this.dailyRevenueInMonth.map((item) =>
      item.dayOfMonth.toString()
    );
    const revenues = this.dailyRevenueInMonth.map((item) => item.dailyRevenue);

    // Tạo options mới thay vì dùng spread operator
    this.chartOptions = {
      title: {
        text: 'Doanh thu theo ngày',
        left: 'center',
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow',
        },
        formatter: (params: any) => {
          const data = params[0].data;
          return `Ngày ${data[0]}: ${this.utilService.formatVND(data[1])}`;
        },
      },
      xAxis: {
        type: 'category',
        data: days,
        axisLabel: {
          rotate: 45,
        },
      },
      yAxis: {
        type: 'value',
        name: 'Doanh thu (VND)',
        axisLabel: {
          formatter: (value: number) => {
            return this.utilService.formatVND(value);
          },
        },
      },
      series: [
        {
          name: 'Doanh thu',
          type: 'bar',
          data: revenues.map((rev, index) => [days[index], rev]),
          itemStyle: {
            color: '#1890ff',
          },
        },
      ],
      grid: {
        containLabel: true,
        left: '3%',
        right: '4%',
        bottom: '3%',
        top: '15%',
      },
    };
  }

  generateYears(): number[] {
    const currentYear = new Date().getFullYear();
    return Array.from({ length: 6 }, (_, i) => currentYear - 3 + i);
  }

  refeshMonthAndYear() {
    const currentDate = new Date();
    this.month = currentDate.getMonth() + 1;
    this.year = currentDate.getFullYear();
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
