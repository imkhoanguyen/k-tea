import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { OrderAddInStore } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  // getPagination(prm: CategoryParams) {
  //   let params = new HttpParams();

  //   if (prm.search.length > 0) {
  //     params = params.append('search', prm.search);
  //   }

  //   if (prm.orderBy.length > 0) {
  //     params = params.append('orderBy', prm.orderBy);
  //   }

  //   params = params.append('pageIndex', prm.pageIndex);
  //   params = params.append('pageSize', prm.pageSize);

  //   return this.http.get<Pagination<Category>>(this.apiUrl + 'categories', {
  //     params,
  //   });
  // }

  addOrderInStore(order: OrderAddInStore) {
    return this.http.post(this.apiUrl + 'orders/in-store', order);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `orders/${id}`);
  }
}
