import { locationsApi, locationsQueryOptions } from "@/entities/locations/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useUpdateLocationPreview(locationId: string) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (previewId: string) => locationsApi.updateLocationPreview(locationId, previewId),
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [locationsQueryOptions.baseKey],
      }),
    onError: (error) => {
      if (error instanceof EnvelopeError) {
        toast.error(error.message);
        return;
      }

      toast.error("Failed to update location preview!");
    },
    onSuccess: () => {
      toast.success("Location preview updated successfully!");
    },
  });

  return {
    updateLocationPreview: mutation.mutate,
    updateLocationPreviewAsync: mutation.mutateAsync,
    isError: mutation.isError,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isPending: mutation.isPending,
  };
}
