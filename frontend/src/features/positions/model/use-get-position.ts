import { positionsQueryOptions } from "@/entities/positions/api";
import { Position } from "@/entities/positions/types";
import { useQuery } from "@tanstack/react-query";

export function useGetPosition(positionId: string) {
  const { data, isPending, error, isError, refetch } = useQuery({
    ...positionsQueryOptions.getPositionOptions(positionId),
    select: (response: any): Position | undefined => {
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