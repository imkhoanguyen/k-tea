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
