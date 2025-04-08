import { PaginationRequest } from './base';

export interface Discount {
  id: number;
  name: string;
  promotionCode: string;
  amountOff?: number;
  percentOff?: number;
}
