import { positionsApi, positionsQueryOptions } from "@/entities/positions/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useUpdatePositionDepartments() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: positionsApi.updatePositionDepartments,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [positionsQueryOptions.baseKey],
      }),
    onError: (error) => {
      if (error instanceof EnvelopeError) {
        toast.error(error.message);
        return;
      }

      toast.error("Failed to update position departments!");
    },
    onSuccess: () => {
      toast.success("Position departments updated successfully!");
    },
  });

  return {
    updatePositionDepartments: mutation.mutate,
    isError: mutation.isError,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isPending: mutation.isPending,
  };
}
