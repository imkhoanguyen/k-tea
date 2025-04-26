import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RecommendDrink } from '../../shared/models/gemini';

@Injectable({
  providedIn: 'root',
})
export class GeminiService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getRecommendDrinkList(input: string) {
    let params = new HttpParams();

    params = params.append('input', input);

    return this.http.get<RecommendDrink[]>(this.apiUrl + 'geminiai', {
      params,
    });
  }
}
