import { PaginationRequest } from './base';
import { Category } from './category';
import { Size } from './size';

export interface Item {
  id: number;
  name: string;
  description: string;
  slug: string;
  imgUrl: string;
  isPublished: boolean;
  isFeatured: boolean;
  categories: Category[];
  sizes: Size[];
}

export class ItemParams extends PaginationRequest {
  constructor() {
    super();
  }
  override pageSize: number = 12;
  categoryId: number = 0;
  isFeatured?: boolean;
}
