import { locationsQueryOptions } from "@/entities/locations/api";
import { useInfiniteQuery } from "@tanstack/react-query";
import { LocationDictionaryState } from "./location-filters-store";

export function useLocationDictionary(filter: LocationDictionaryState) {
  const {
    data,
    isPending,
    error,
    isError,
    refetch,
    fetchNextPage,
    isFetchingNextPage,
    hasNextPage,
  } = useInfiniteQuery({
    ...locationsQueryOptions.getLocationDictionaryInfinityOptions({
      ...filter,
    }),
  });
  return {
    items: data?.items,
    totalPages: data?.totalPages ?? undefined,
    currentPage: data?.page,
    isPending,
    error,
    isError,
    refetch,
    isFetchingNextPage,
    hasNextPage,
    fetchNextPage,
  };
}