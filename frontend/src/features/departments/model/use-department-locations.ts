import { departmentsApi, departmentsQueryOptions } from "@/entities/departments/api";
import { EnvelopeError } from "@/shared/api/errors";
import { useQueryClient, useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

export function useUpdateDepartmentLocations() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: departmentsApi.updateDepartmentLocations,
    onSettled: () =>
      queryClient.invalidateQueries({
        queryKey: [departmentsQueryOptions.baseKey],
      }),
    onError: (error) => {
      if (error instanceof EnvelopeError) {
        toast.error(error.message);
        return;
      }

      toast.error("Failed to update department locations!");
    },
    onSuccess: () => {
      toast.success("Department locations updated successfully!");
    },
  });

  return {
    updateDepartmentLocations: mutation.mutate,
    isError: mutation.isError,
    error: mutation.error instanceof EnvelopeError ? mutation.error : undefined,
    isPending: mutation.isPending,
  };
}