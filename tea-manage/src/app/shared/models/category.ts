import { PaginationRequest } from './base';

export interface Category {
  id: number;
  name: string;
  description: string;
  slug: string;
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

export interface CategoryAddChildren {
  name: string;
  description: string;
  slug: string;
  parentId: number;
}

export interface CategoryUpdate {
  id: number;
  name: string;
  description: string;
  slug: string;
}
