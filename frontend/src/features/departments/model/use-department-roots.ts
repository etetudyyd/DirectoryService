import { departmentsQueryOptions } from "@/entities/departments/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useInfiniteQuery } from "@tanstack/react-query";

export function useDepartmentRoots() {
  const {
    data,
    isPending,
    error,
    isError,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  } = useInfiniteQuery({
    ...departmentsQueryOptions.getRootDepartmentsInfinityOptions(),
  });

  return {
    departments: data?.items,
    isPending,
    error,
    isError,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  };
}