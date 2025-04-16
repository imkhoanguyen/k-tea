export interface PaymentResponse {
  total: number;
  username: string;
  discountId?: number;
  discountPrice?: number;
  phoneNumber: string;
  address: string;
  description?: string;
}

export interface PaymentRequest {
  responseCode: string;
  username: string;
  discountId?: number;
  discountPrice?: number;
  phoneNumber: string;
  address: string;
  description?: string;
}
