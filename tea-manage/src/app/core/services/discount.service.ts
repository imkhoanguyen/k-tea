import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  Discount,
  DiscountAdd,
  DiscountParams,
  DiscountUpdate,
} from '../../shared/models/discount';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class DiscountService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getPagination(prm: DiscountParams) {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<Discount>>(this.apiUrl + 'discounts', {
      params,
    });
  }

  add(d: DiscountAdd) {
    return this.http.post(this.apiUrl + 'discounts', d);
  }

  checkDiscount(code: string) {
    return this.http.get(this.apiUrl + `discounts/${code}`);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `discounts/${id}`);
  }

  update(d: DiscountUpdate) {
    return this.http.put(this.apiUrl + `discounts/${d.id}`, d);
  }

  delete(id: number) {
    return this.http.delete(this.apiUrl + `discounts/${id}`);
  }

  deletes(discountIdList: number[]) {
    let params = new HttpParams();
    discountIdList.forEach((id) => {
      params = params.append('discountIdList', id);
    });

    return this.http.delete(this.apiUrl + `discounts`, { params });
  }
}
