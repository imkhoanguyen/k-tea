import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  Category,
  CategoryAddChildren,
  CategoryAddParent,
  CategoryParams,
  CategoryUpdate,
} from '../../shared/models/category';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getPagination(prm: CategoryParams) {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<Category>>(this.apiUrl + 'categories', {
      params,
    });
  }

  addParent(c: CategoryAddParent) {
    return this.http.post(this.apiUrl + 'categories', c);
  }

  addChildren(c: CategoryAddChildren) {
    return this.http.post(this.apiUrl + 'categories/children', c);
  }

  update(c: CategoryUpdate) {
    return this.http.put(this.apiUrl + `categories/${c.id}`, c);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `categories/${id}`);
  }

  delete(id: number) {
    return this.http.delete(this.apiUrl + `categories/${id}`);
  }

  deletes(categoryIdList: number[]) {
    let params = new HttpParams();
    categoryIdList.forEach((id) => {
      params = params.append('categoryIdList', id);
    });

    return this.http.delete(this.apiUrl + `categories`, { params });
  }
}
