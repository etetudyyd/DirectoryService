import { positionsApi, positionsQueryOptions } from "@/entities/positions/api";
import { EnvelopeError } from "@/shared/api/errors";
import { queryClient } from "@/shared/api/query-client";
import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

export function useCreatePosition() {
  const mutation = useMutation({
    mutationFn: positionsApi.createPosition,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [positionsQueryOptions.baseKey],
      }),
    onSuccess: () => {
      toast.success("Position created successfully");
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
    createPosition: mutation.mutate,
    isPending: mutation.isPending,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isError: mutation.isError,
  };
}