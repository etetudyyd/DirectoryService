import { useDebounce } from "use-debounce";
import { PositionsFilterState } from "./position-filters-store";
import { positionsQueryOptions } from "@/entities/positions/api";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";
import { EnvelopeError } from "@/shared/api/errors";


export const PAGE_SIZE = 5;

export function usePositionsList({ search, pageSize, isActive }: PositionsFilterState) {
  
  const [debouncedSearch] = useDebounce(search, 300);

  const {
    data,
    isPending,
    error,
    isError,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    ...positionsQueryOptions.getPositionsInfinityOptions({
      search: debouncedSearch,
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
    positions: data?.items,
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
