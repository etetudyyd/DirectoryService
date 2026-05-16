import { departmentsQueryOptions } from "@/entities/departments/api";
import { EnvelopeError } from "@/shared/api/errors";
import useCursorRef from "@/shared/hooks/use-cursor-ref";
import { useInfiniteQuery } from "@tanstack/react-query";

export function useGetChildrenDepartment(parentId: string) {
  const {
    data,
    isPending,
    error,
    isError,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    ...departmentsQueryOptions.getChildrenDepartmentsInfinityOptions(parentId),
  });

  const cursorRef = useCursorRef({
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  });

  return {
    departments: data?.items ?? [],
    totalPages: data?.totalPages ?? 0,
    totalItems: data?.totalItems ?? 0,
    isPending,
    error: error instanceof EnvelopeError ? error : undefined,
    isError,
    cursorRef,
    isFetchingNextPage,
  };
}