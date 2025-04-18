import { PaginationRequest } from './base';

export interface OrderAddInStore {
  createdById: string;
  discountPrice?: number;
  discountId?: number;
  paymentType: string;
  items: OrderItemAdd[];
}

export interface OrderItemAdd {
  itemName: string;
  price: number;
  itemSize: string;
  quantity: number;
  itemId: number;
  itemImg: string;
}

export interface OrderList {
  id: number;
  orderStatus: string;
  orderType: string;
  paymentStatus: string;
  paymentType: string;
  created: string;
  subTotal: number;
  discountPrice: number | null;
  shippingFee: number | null;
  discountId: number | null;
  description: string | null;
  userId: string | null;
  customerAddress: string | null;
  customerName: string | null;
  customerPhone: string | null;
  createdById: string | null;
  total: number;
}

export class OrderParams extends PaginationRequest {
  constructor() {
    super();
  }
  fromDate?: Date;
  toDate?: Date;
  minAmount?: number;
  maxAmount?: number;
  userName?: string;
}
