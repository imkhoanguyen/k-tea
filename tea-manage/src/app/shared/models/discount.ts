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
