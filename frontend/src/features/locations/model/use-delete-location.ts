import { locationsApi, locationsQueryOptions } from "@/entities/locations/api";
import { EnvelopeError } from "@/shared/api/errors";
import { queryClient } from "@/shared/api/query-client";
import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

export function useDeleteLocation() {
  const mutation = useMutation({
    mutationFn: locationsApi.deleteLocation,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [locationsQueryOptions.baseKey],
      }),
    onSuccess: () => {
      toast.success("Location deleted successfully");
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
    deleteLocation: mutation.mutate,
    isPending: mutation.isPending,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isError: mutation.isError,
  };
}
