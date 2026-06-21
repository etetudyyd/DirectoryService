import { departmentsQueryOptions } from "@/entities/departments/api";
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
    page: data?.page,
    totalPages: data?.totalPages,
    totalItems: data?.totalItems,
    isPending,
    error,
    isError,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  };
}
