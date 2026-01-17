import { apiClient } from "@/shared/api/axios-instance";
import { Address, Location } from "./types";
import { PaginationResponse } from "@/shared/api/types";
import { queryOptions } from "@tanstack/react-query";

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

export type Envelope<T = unknown> = {
    result: T | null;
    error: ApiError | null;
    isError: boolean;
    timeGenerated: string;
};

export type ApiError = {
    messages: ErrorMessage[];
    type: ErrorType;
};

export type ErrorMessage = {
    code: string;
    message: string;
    invalidField?: string | null;
};

export type ErrorType = 
    | "validation" 
    | "not_found" 
    | "failure" 
    | "conflict"
    | "authentication"
    | "authorization";

export const locationsApi = {
    getLocations: async (request: GetLocationsRequest) => {
        const response = await apiClient.get<Envelope<PaginationResponse<Location>>>("/locations", { params: request });

        return response.data.result;
    },
    createLocation: async (request: CreateLocationRequest) => {
        const response = await apiClient.post<Envelope<Location>>("/locations/create", request);

        return response.data.result;
    },
};

export const locationsQueryOptions = {
    baseKey: "locations",

    getLocationsOptions: ({
        page,
        pageSize
    } : {
        page: number,
        pageSize: number
    }) => {
        return queryOptions({
            queryKey: [locationsQueryOptions.baseKey, {page}],
            queryFn: () => locationsApi.getLocations({ page: page, pageSize: pageSize }),
        });
    },
};