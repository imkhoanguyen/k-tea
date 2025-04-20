import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  OrderAddInStore,
  OrderAddOnline,
  OrderList,
  OrderParams,
} from '../../shared/models/order';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getPagination(prm: OrderParams) {
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

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<OrderList>>(this.apiUrl + 'orders', {
      params,
    });
  }

  addOrderInStore(order: OrderAddInStore) {
    return this.http.post(this.apiUrl + 'orders/in-store', order);
  }

  addOrderOnline(order: OrderAddOnline) {
    return this.http.post(this.apiUrl + 'orders/online', order);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `orders/${id}`);
  }
}
