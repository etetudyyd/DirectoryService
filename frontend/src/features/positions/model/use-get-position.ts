import { positionsQueryOptions } from "@/entities/positions/api";
import { useQuery } from "@tanstack/react-query";

export function useGetPosition(positionId: string) {
  const { data, isPending, error, isError, refetch } = useQuery({
    ...positionsQueryOptions.getPositionOptions(positionId),
    select: (response: any) => {
      return response?.position;
    }
  });

  return {
    position: data,
    isPending,
    error,
    isError,
    refetch,
  };
}