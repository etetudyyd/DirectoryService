import { locationsQueryOptions } from "@/entities/locations/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useQuery } from "@tanstack/react-query";

const PAGE_SIZE = 3;

export function useLocationsList({ page }: { page: number }) {
    const { data, isLoading, error, isError} = useQuery(
        locationsQueryOptions.getLocationsOptions({ page, pageSize: PAGE_SIZE })
);

    return { 
        locations: data?.items,
        totalPages: data?.totalPages,
        totalItems: data?.totalItems,
        isLoading,
        error: error instanceof EnvelopeError ? error : undefined, 
        isError,
    }; 
}