export interface Size {
  id: number;
  name: string;
  description: string;
  price: number;
  newPrice: number;
}

export interface SizeAdd {
  name: string;
  description: string;
  price: number;
  newPrice: number;
}

export interface SizeUpdate {
  id: number;
  name: string;
  description: string;
  price: number;
  newPrice: number;
  itemId: number;
}
