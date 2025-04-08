import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Discount } from '../../shared/models/discount';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class DiscountService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  checkDiscount(code: string) {
    return this.http.get(this.apiUrl + `discounts/${code}`);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `discounts/${id}`);
  }
}
