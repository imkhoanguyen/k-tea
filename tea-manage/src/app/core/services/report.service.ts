import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  DailyRevenueInMonth,
  Report,
  TopSellingItem,
} from '../../shared/models/report';
import { Observable } from 'rxjs';
import { OrderParams } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getReport() {
    return this.http.get<Report>(this.apiUrl + 'reports');
  }

  getTopSellingItem(topCount: number) {
    let params = new HttpParams();
    params = params.append('topCount', topCount);
    return this.http.get<TopSellingItem[]>(
      this.apiUrl + 'reports/get-top-selling-items',
      { params }
    );
  }

  getDailyRevenueInMonth(month: number, year: number) {
    let params = new HttpParams();
    params = params.append('month', month);
    params = params.append('year', year);

    return this.http.get<DailyRevenueInMonth[]>(
      this.apiUrl + 'reports/get-daily-revenue-in-month',
      { params }
    );
  }

  // order.service.ts
  exportPdf(orderId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}reports/print?orderId=${orderId}`, {
      responseType: 'blob',
    });
  }

  exportExcel(prm: OrderParams): Observable<Blob> {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    if (prm.fromDate) {
      params = params.append('fromDate', prm.fromDate.toISOString());
    }

    if (prm.toDate) {
      params = params.append('toDate', prm.toDate.toISOString());
    }

    if (prm.minAmount !== undefined && prm.minAmount !== null) {
      params = params.append('minAmount', prm.minAmount.toString());
    }

    if (prm.maxAmount !== undefined && prm.maxAmount !== null) {
      params = params.append('maxAmount', prm.maxAmount.toString());
    }

    if (prm.userName && prm.userName.length > 0) {
      params = params.append('userName', prm.userName);
    }

    params = params.append('pageIndex', 1);
    params = params.append('pageSize', 1);

    return this.http.get(`${this.apiUrl}reports/export`, {
      params,
      responseType: 'blob',
    });
  }
}
