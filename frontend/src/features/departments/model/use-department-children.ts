import { departmentsQueryOptions } from "@/entities/departments/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useInfiniteQuery } from "@tanstack/react-query";

export function useDepartmentChildren(
  parentId: string,
  options?: {
    enabled?: boolean;
  },
) {
  const {
    data,
    isPending,
    error,
    isError,

    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    ...departmentsQueryOptions.getChildrenDepartmentsInfinityOptions(
      parentId,
    ),

    enabled: options?.enabled,
  });

  return {
    departments:
      data?.items ?? [],
    totalPages:
      data?.totalPages ?? 0,
    totalItems:
      data?.totalItems ?? 0,
    isPending,
    error:
      error instanceof EnvelopeError
        ? error
        : undefined,

    isError,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  };
}