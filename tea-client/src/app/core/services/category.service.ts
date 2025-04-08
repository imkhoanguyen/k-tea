import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<Category[]>(this.apiUrl + `categories/all`);
  }
}
