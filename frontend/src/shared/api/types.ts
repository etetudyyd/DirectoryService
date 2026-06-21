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

export const normalizePaginationResponse = <T>(
  response: PaginationResponse<T> & {
    departments?: T[];
    positions?: T[];
    locations?: T[];
  },
): PaginationResponse<T> => ({
  ...response,
  items:
    response.items ??
    response.departments ??
    response.positions ??
    response.locations ??
    [],
  totalItems: response.totalItems ?? 0,
  page: response.page ?? 1,
  pageSize: response.pageSize ?? PAGE_SIZE,
  totalPages: response.totalPages ?? 1,
  parentId: response.parentId ?? "",
  sortBy: response.sortBy ?? "",
  sortDirection: response.sortDirection ?? "",
});

export const PAGE_SIZE = 5;

export const PREFETCH_CHILDREN_LIMIT = 5;

export const PREFETCH = 3;
