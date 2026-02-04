import { apiClient } from "@/shared/api/axios-instance";
import { DictionaryItemResponse, DictionaryResponse, PaginationResponse } from "@/shared/api/types";
import { Envelope } from "@/shared/api/envelope";
import routes from "@/shared/routes";
import { infiniteQueryOptions } from "@tanstack/react-query";
import {
  DepartmentDictionaryState,
  GetDepartmentsDictionaryRequest,
} from "./types";

export const departmentsApi = {
  getDepartmentsDictionary: async (
    request: GetDepartmentsDictionaryRequest,
  ) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<DictionaryItemResponse>>
    >(`${routes.departments}/dictionary`, {
      params: request
    });

    return response.data.result;
  },
};

export const departmentsQueryOptions = {
  baseKey: "departments",

  getDepartmentDictionaryInfinityOptions: (
    filter: DepartmentDictionaryState,
  ) => {
    return infiniteQueryOptions({
      queryFn: ({ pageParam }) => {
        return departmentsApi.getDepartmentsDictionary({
          ...filter,
          page: pageParam,
        });
      },
      queryKey: [departmentsQueryOptions.baseKey, filter],
      initialPageParam: 1,
      getNextPageParam: (response) => {
        return !response || response.page >= response.totalPages
          ? undefined
          : response.page + 1;
      },

      select: (data): PaginationResponse<DictionaryItemResponse> => ({
        items: data.pages.flatMap((page) => page?.items ?? []),
        totalItems: data.pages[0]?.totalItems ?? 0,
        page: data.pages[0]?.page ?? 1,
        pageSize: data.pages[0]?.pageSize ?? filter.pageSize,
        totalPages: data.pages[0]?.totalPages ?? 0,
      }),
    });
  },
};