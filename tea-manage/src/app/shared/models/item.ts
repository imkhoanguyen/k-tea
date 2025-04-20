import { PaginationRequest } from './base';
import { Category } from './category';
import { Size, SizeAdd } from './size';

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

export interface ItemAdd {
  name: string;
  description: string;
  slug: string;
  isPublished: boolean;
  imgFile: string;
  categoryIdList: number[];
  sizeCreateRequestJson: SizeAdd;
}

export interface ItemUpdate {
  id: number;
  name: string;
  slug: string;
  description: string;
  categoryIdList: number[];
  isPublished: boolean;
  isFeatured: boolean;
}

export class ItemParams extends PaginationRequest {
  constructor() {
    super();
  }
  override pageSize: number = 6;
}
