export type PaginationResponse<T> = {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  parentId: string;
  sortBy: string;
  sortDirection: string;
};

export type DictionaryItemResponse = {
  id: string;
  name: string;
};
