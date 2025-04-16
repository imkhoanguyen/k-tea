import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaymentRequest, PaymentResponse } from '../../shared/models/payment';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paymentSuccessed = false;
  paymentFailed = false;

  getPaymentUrl(response: PaymentResponse) {
    return this.http.post<string>(this.baseUrl + 'payments', response);
  }

  getPaymentReturn(paymentRequest: PaymentRequest) {
    return this.http.post<any>(
      this.baseUrl + 'payments/return',
      paymentRequest
    );
  }
}
