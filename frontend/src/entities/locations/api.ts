import { apiClient } from "@/shared/api/axios-instance";
import { Address, Location } from "./types";
import { PaginationResponse } from "@/shared/api/types";
import { queryOptions } from "@tanstack/react-query";
import { Envelope } from "@/shared/api/envelope";

export type CreateLocationRequest = {
  name: string;
  address: Address;
  timeZone: string;
  departmentsIds: string[];
};

export type GetLocationsRequest = {
  search?: string;
  ids?: string[];
  isActive?: boolean;
  page?: number;
  pageSize?: number;
};

export const locationsApi = {
  getLocations: async (request: GetLocationsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Location>>
    >("/locations", { params: request });

    return response.data.result;
  },

  createLocation: async (request: CreateLocationRequest) => {
    const response = await apiClient.post<Envelope<Location>>(
      "/locations",
      request,
    );
    return response.data;
  },
};

export const locationsQueryOptions = {
  baseKey: "locations",

  getLocationsOptions: ({
    page,
    pageSize,
  }: {
    page: number;
    pageSize: number;
  }) => {
    return queryOptions({
      queryKey: [locationsQueryOptions.baseKey, { page }],
      queryFn: () =>
        locationsApi.getLocations({ page: page, pageSize: pageSize }),
    });
  },
};
