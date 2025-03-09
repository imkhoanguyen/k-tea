import { PaginationRequest } from './base';

export interface Category {
  id: number;
  name: string;
  description: string;
  slug: string;
  children: Category[];
}

export class CategoryParams extends PaginationRequest {
  constructor() {
    super();
  }
}

export interface CategoryAddParent {
  name: string;
  description: string;
  slug: string;
}

export interface CategoryUpdate {
  id: number;
  name: string;
  description: string;
  slug: string;
}
