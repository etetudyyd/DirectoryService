import { apiClient } from "@/shared/api/axios-instance";
import { DictionaryItemResponse, PaginationResponse } from "@/shared/api/types";
import { Envelope } from "@/shared/api/envelope";
import routes from "@/shared/routes";
import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";
import {
  Department,
  DepartmentDetails,
  
} from "./types";
import { DepartmentDictionaryState, DepartmentsFilterState } from "@/features/departments/model/department-filters-store";

export type GetDepartmentsDictionaryRequest = {
  search?: string;
  page: number;
  pageSize: number;
}

export type GetDepartmentsRequest = {
  search?: string;
  ids?: string[];
  parentId?: string; 
  isActive?: boolean;
  page?: number;
  pageSize?: number;
  sortBy: string;
  sortDirection: string;
}

export type GetRootDepartmentsRequest = {
  page: number;
  pageSize: number;
  prefetch: number;
}

export type GetChildrenDepartmentsRequest = {
  parentId: string;
  page: number;
  pageSize: number;
}

export type UpdateDepartmentLocationsRequest = {
  departmentsId: string;
  locationsIds: string[];
};

export type CreateDepartmentRequest = {
    name: string;
    identifier: string;
    parentId?: string | null;
    locationsIds: string[];
}

export type UpdateDepartmentRequest = {
  departmentId: string;
  name: string;
  identifier: string;
}

export const departmentsApi = {

getRootDepartments: async (request: GetRootDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Department>>
    >(routes.departments, { params: request });

    return response.data.result;
  },

  getChildrenDepartments: async (request: GetChildrenDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Department>>
    >(routes.departments, { params: request });

    return response.data.result;
  },

   getDepartments: async (request: GetDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Department>>
    >(routes.departments, { params: request });

    return response.data.result;
  },

  getDepartmentById: async (departmentId: string) => {
    const response = await apiClient.get<Envelope<DepartmentDetails>>(
      `${routes.departments}/${departmentId}`,
    );
    return response.data.result;
  },

  createDepartment: async (request: CreateDepartmentRequest) => {
    const response = await apiClient.post<Envelope<Department>>(
      `${routes.departments}/create`,
      request,
    );
    return response.data;
  },

  updateDepartment: async ({
    departmentId,
    name,
    identifier,
  }: UpdateDepartmentRequest): Promise<Envelope<Department>> => {
    const response = await apiClient.patch<Envelope<Department>>(
      `${routes.departments}/${departmentId}`,
      { name, identifier },
    );
    return response.data;
  },

  updateDepartmentLocations: async (
    request: UpdateDepartmentLocationsRequest,
  ) => {
    const response = await apiClient.put<Envelope<Department>>(
      `${routes.positions}/${request.departmentsId}${routes.locations}`,
      request,
    );
    return response.data;
  },

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

  deleteDepartment: async (departmentId: string) => {
    const response = await apiClient.delete<Envelope<Department>>(
      `${routes.departments}/${departmentId}`,
    );
    return response.data;
  },
};

export const departmentsQueryOptions = {
  baseKey: "departments",

   getDepartmentOptions: (departmentId: string) => {
    return queryOptions({
      queryKey: [departmentsQueryOptions.baseKey, departmentId],
      queryFn: () => departmentsApi.getDepartmentById(departmentId),
    });
  },

  getDepartments: async (request: GetDepartmentsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Department>>
    >(routes.departments, { params: request });

    return response.data.result;
  },

  getRootDepartmentsOptions: ({
      page,
      pageSize,
      prefetch
    }: {
      page: number;
      pageSize: number;
      prefetch: number;
    }) => {
      return queryOptions({
        queryKey: [departmentsQueryOptions.baseKey, { page }],
        queryFn: () =>
          departmentsApi.getRootDepartments({ page: page, pageSize: pageSize, prefetch: prefetch }),
      });
    },

    getChildrenDepartmentsOptions: ({
      parentId,
      page,
      pageSize
    }: {
      parentId: string;
      page: number;
      pageSize: number;
    }) => {
      return queryOptions({
        queryKey: [departmentsQueryOptions.baseKey, { page }],
        queryFn: () =>
          departmentsApi.getChildrenDepartments({ parentId: parentId, page: page, pageSize: pageSize }),
      });
    },

    getDepartmentsOptions: ({
        page,
        pageSize,
        parentId,
        sortBy,
        sortDirection
      }: {
        page: number;
        pageSize: number;
        parentId: string | null;
        sortBy: string;
        sortDirection: string;
      }) => {
        return queryOptions({
          queryKey: [departmentsQueryOptions.baseKey, { page }],
          queryFn: () =>
            departmentsApi.getDepartments({ page: page, pageSize: pageSize, parentId: parentId, sortBy: sortBy, sortDirection: sortDirection}),
        });
      },
      
      getDepartmentsInfinityOptions: (filter: DepartmentsFilterState) => {
        return infiniteQueryOptions({
          queryKey: [departmentsQueryOptions.baseKey, filter],
          queryFn: ({ pageParam }) => {
            return departmentsApi.getDepartments({
              ...filter,
              page: pageParam,
            });
          },
          initialPageParam: 1,
          getNextPageParam: (response) => {
            if (!response || response.page >= response.totalPages) return undefined;
            return response.page + 1;
          },
          select: (data): PaginationResponse<Department> => {
            return {
              items: data.pages.flatMap((page) => page?.items ?? []),
              totalItems: data.pages[0]?.totalItems ?? 0,
              page: data.pages[0]?.page ?? 1,
              pageSize: data.pages[0]?.pageSize ?? filter.pageSize,
              totalPages: data.pages[0]?.totalPages ?? 0,
              parentId: data.pages[0]?.parentId ?? filter.parentId,
              sortBy: data.pages[0]?.sortBy ?? filter.sortBy,
              sortDirection: data.pages[0]?.sortDirection ?? filter.sortDirection,
            };
          },
        });
      },

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
            parentId: "",
            sortBy: "",
            sortDirection: "",
          }),
        });
      },
    };