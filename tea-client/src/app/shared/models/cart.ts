export type CartType = {
  id: string;
  items: CartItem[];
};

export type CartItem = {
  itemName: string;
  size: string;
  itemImg: string;
  itemId: number;
  quantity: number;
  price: number;
};

export class Cart implements CartType {
  id: string;
  items: CartItem[] = [];

  constructor() {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      this.id = user.userName || '';
    } else {
      this.id = '';
    }
  }
}
