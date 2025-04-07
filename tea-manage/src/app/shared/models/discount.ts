import { PaginationRequest } from './base';

export interface Discount {
  id: number;
  name: string;
  promotionCode: string;
  amountOff?: number;
  percentOff?: number;
}

export interface DiscountAdd {
  name: string;
  promotionCode: string;
  amountOff?: number;
  percentOff?: number;
}

export interface DiscountUpdate {
  id: number;
  name: string;
  promotionCode: string;
  amountOff?: number;
  percentOff?: number;
}

export class DiscountParams extends PaginationRequest {
  constructor() {
    super();
  }
}
