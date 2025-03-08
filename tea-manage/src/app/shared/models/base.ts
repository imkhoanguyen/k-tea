export class PaginationRequest {
  pageIndex: number = 1;
  pageSize: number = 5;
  search: string = '';
  protected orderBy: string = 'id_desc';
}

export type Pagination<T> = {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: T[];
};
