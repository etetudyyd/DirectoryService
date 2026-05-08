import { useDebounce } from "use-debounce";
import { DepartmentsFilterState } from "./department-filters-store";
import { departmentsQueryOptions } from "@/entities/departments/api";
import { useInfiniteQuery } from "@tanstack/react-query";
import { RefCallback, useCallback } from "react";
import { EnvelopeError } from "@/shared/api/errors";

export function useDepartmentsList({
    locationsIds,
    search,
    pageSize,
    isActive,
    parentId,
    sortBy,
    sortDirection
}: DepartmentsFilterState) {
  
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
    ...departmentsQueryOptions.getDepartmentsInfinityOptions({
      locationsIds,
      search: debouncedSearch,
      isActive,
      pageSize: pageSize,
      parentId,
      sortBy,
      sortDirection
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
      departments: data?.items,
      totalPages: data?.totalPages,
      totalItems: data?.totalItems,
      isPending,
      isActive,
      error: error instanceof EnvelopeError ? error : undefined,
      isError,
      cursorRef,
      isFetchingNextPage
    };
}