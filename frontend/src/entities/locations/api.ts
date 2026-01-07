import { apiCLient } from "@/shared/api/axios-instance";
import { Location } from "./types";

export type CreateLocationRequest = {
    name: string;
    address: string;
    timezone: string;
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
    getLocations: async (request: GetLocationsRequest): Promise<Location[]> => {
        const response = await apiCLient.get<Envelope<{locations: Location[]}>>("/locations", { params: request });

        return response.data.result?.locations || [];
    },
    createLocation: async (request: CreateLocationRequest) => {
        const response = await apiCLient.post("/locations", request);

        return response.data;
    },
};