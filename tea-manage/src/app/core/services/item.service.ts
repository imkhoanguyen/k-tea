import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  ImportResult,
  Item,
  ItemParams,
  ItemUpdate,
} from '../../shared/models/item';
import { Pagination } from '../../shared/models/base';
import { SizeAdd, SizeUpdate } from '../../shared/models/size';
import { Observable } from 'rxjs';

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

  add(fdt: FormData) {
    return this.http.post(this.apiUrl + 'items', fdt);
  }

  addSizes(itemId: number, sizes: SizeAdd[]) {
    return this.http.post(this.apiUrl + `items/${itemId}/sizes`, sizes);
  }

  update(i: ItemUpdate) {
    return this.http.put(this.apiUrl + `items/${i.id}`, i);
  }

  updateSize(s: SizeUpdate) {
    return this.http.put(this.apiUrl + `items/${s.itemId}/size`, s);
  }

  updateImage(itemId: number, fdt: FormData) {
    return this.http.put(this.apiUrl + `items/${itemId}/image`, fdt);
  }

  get(id: number) {
    return this.http.get(this.apiUrl + `items/${id}`);
  }

  delete(id: number) {
    return this.http.delete(this.apiUrl + `items/${id}`);
  }

  deletes(itemIdList: number[]) {
    let params = new HttpParams();
    itemIdList.forEach((id) => {
      params = params.append('itemIdList', id);
    });

    return this.http.delete(this.apiUrl + `items`, { params });
  }

  deleteSizes(itemId: number, idList: number[]) {
    let params = new HttpParams();
    idList.forEach((id) => {
      params = params.append('sizeIdList', id);
    });
    return this.http.delete(this.apiUrl + `items/${itemId}/sizes`, { params });
  }

  exportTemplateUpdate(ids: number[]): Observable<Blob> {
    return this.http.post(`${this.apiUrl}items/export-template-update`, ids, {
      responseType: 'blob',
    });
  }

  importUpdateItem(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImportResult>(
      `${this.apiUrl}items/import-update-items`,
      formData
    );
  }
}
