import { locationsQueryOptions } from "@/entities/locations/api";
import { useQuery } from "@tanstack/react-query";

const PAGE_SIZE = 3;

export function useLocationsList({ page }: { page: number }) {
    const { data, isLoading, error} = useQuery(
        locationsQueryOptions.getLocationsOptions({ page, pageSize: PAGE_SIZE })
);

    return { 
        locations: data?.items,
        totalPages: data?.totalPages,
        totalItems: data?.totalItems,
        isLoading,
        error 
    }; 
}