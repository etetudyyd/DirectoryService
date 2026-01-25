import { locationsQueryOptions } from "@/entities/locations/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";
import { LocationsFilter } from "../locations-list";

export const PAGE_SIZE = 5;

export function useLocationsList({ search, pageSize, isActive }: LocationsFilter) {
  const {
    data,
    isPending,
    error,
    isError,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    ...locationsQueryOptions.getLocationsInfinityOptions({
      search,
      isActive,
      pageSize: pageSize,
    }),
  });

  const cursorRef: RefCallback<HTMLDivElement> = useCallback(
    (el) => {
      const observer = new IntersectionObserver(
        (entries) => {
          if (entries[0].isIntersecting && !isFetchingNextPage && hasNextPage) {
            fetchNextPage();
          }
        },
        { threshold: 0.5 },
      );

      if (el) {
        observer.observe(el);

        return () => observer.disconnect();
      }
    },
    [fetchNextPage, hasNextPage, isFetchingNextPage],
  );

  return {
    locations: data?.items,
    totalPages: data?.totalPages,
    totalItems: data?.totalItems,
    isPending,
    isActive,
    error: error instanceof EnvelopeError ? error : undefined,
    isError,
    cursorRef,
    isFetchingNextPage,
  };
}
