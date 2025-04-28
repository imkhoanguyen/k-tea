import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  DailyRevenueInMonth,
  Report,
  TopSellingItem,
} from '../../shared/models/report';
import { Observable } from 'rxjs';

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
}
