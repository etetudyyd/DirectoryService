import { positionsApi, positionsQueryOptions } from "@/entities/positions/api";
import { EnvelopeError } from "@/shared/api/errors";
import { queryClient } from "@/shared/api/query-client";
import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

export function useDeletePosition() {
  const mutation = useMutation({
    mutationFn: positionsApi.deletePosition,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [positionsQueryOptions.baseKey],
      }),
    onSuccess: () => {
      toast.success("Position deleted successfully");
    },
    onError: (error) => {
      if (error instanceof EnvelopeError) {
        toast.error(error.firstMessage);
        return;
      }

      toast.error("An unexpected error occurred");
    },
  });

  return {
    deletePosition: mutation.mutate,
    isPending: mutation.isPending,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isError: mutation.isError,
  };
}