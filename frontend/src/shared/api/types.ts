export type PaginationResponse<T> = {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
};

export type DictionaryResponse = {
  items: DictionaryItemResponse[];
};

export type DictionaryItemResponse = {
  id: string;
  name: string;
};
