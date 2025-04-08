import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Item, ItemParams } from '../../shared/models/item';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class ItemService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getPagination(prm: ItemParams) {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<Item>>(this.apiUrl + 'items', {
      params,
    });
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `items/${id}`);
  }
}
