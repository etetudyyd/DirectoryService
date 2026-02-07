import { apiClient } from "@/shared/api/axios-instance";
import { Position, PositionDetails } from "./types";
import { Envelope } from "@/shared/api/envelope";
import { PaginationResponse } from "@/shared/api/types";
import { infiniteQueryOptions, queryOptions } from "@tanstack/react-query";
import { PositionsFilterState } from "@/features/positions/model/position-filters-store";
import routes from "@/shared/routes";

export type UpdatePositionRequest = {
  positionId: string;
  name: string;
  description: string;
};

export type CreatePositionRequest = {
  name: string;
  description: string;
  departmentsIds: string[];
};

export type GetPositionsRequest = {
  search?: string;
  ids?: string[];
  isActive?: boolean;
  page?: number;
  pageSize?: number;
};

export const positionsApi = {
  getPositions: async (request: GetPositionsRequest) => {
    const response = await apiClient.get<
      Envelope<PaginationResponse<Position>>
    >(routes.positions, { params: request });

    return response.data.result;
  },

  getPositionById: async (positionId: string) => {
    const response = await apiClient.get<Envelope<PositionDetails>>(
      `${routes.positions}/${positionId}`,
    );
    return response.data.result;
  },

  createPosition: async (request: CreatePositionRequest) => {
    const response = await apiClient.post<Envelope<Position>>(
      routes.positions,
      request,
    );
    return response.data;
  },

  updatePosition: async ({
    positionId,
    name,
    description,
  }: UpdatePositionRequest): Promise<Envelope<Position>> => {
    const response = await apiClient.patch<Envelope<Position>>(
      `${routes.positions}/${positionId}`,
      { name, description },
    );
    return response.data;
  },

  deletePosition: async (positionId: string) => {
    const response = await apiClient.delete<Envelope<Position>>(
      `${routes.positions}/${positionId}`,
    );
    return response.data;
  },
};

export const positionsQueryOptions = {
  baseKey: "positions",

  getPositionOptions: (positionId: string) => {
    return queryOptions({
      queryKey: [positionsQueryOptions.baseKey, positionId],
      queryFn: () => positionsApi.getPositionById(positionId),
    });
  },

  getPositionsOptions: ({
    page,
    pageSize,
  }: {
    page: number;
    pageSize: number;
  }) => {
    return queryOptions({
      queryKey: [positionsQueryOptions.baseKey, { page }],
      queryFn: () =>
        positionsApi.getPositions({ page: page, pageSize: pageSize }),
    });
  },
  getPositionsInfinityOptions: (filter: PositionsFilterState) => {
    return infiniteQueryOptions({
      queryKey: [positionsQueryOptions.baseKey, filter],
      queryFn: ({ pageParam }) => {
        return positionsApi.getPositions({
          ...filter,
          page: pageParam,
        });
      },
      initialPageParam: 1,
      getNextPageParam: (response) => {
        if (!response || response.page >= response.totalPages) return undefined;
        return response.page + 1;
      },
      select: (data): PaginationResponse<Position> => {
        return {
          items: data.pages.flatMap((page) => page?.items ?? []),
          totalItems: data.pages[0]?.totalItems ?? 0,
          page: data.pages[0]?.page ?? 1,
          pageSize: data.pages[0]?.pageSize ?? filter.pageSize,
          totalPages: data.pages[0]?.totalPages ?? 0,
        };
      },
    });
  },
};
