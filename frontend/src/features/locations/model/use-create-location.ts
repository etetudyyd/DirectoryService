import { locationsApi, locationsQueryOptions } from "@/entities/locations/api";
import { queryClient } from "@/shared/api/query-client";
import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

export function useCreateLocation() {
const mutation = useMutation({
    mutationFn: locationsApi.createLocation,
    onSettled: () => queryClient.invalidateQueries({
         queryKey: [locationsQueryOptions.baseKey] }),
    onSuccess: () => {
        toast.success("Location created successfully");
    },
    onError: (error) => {
        toast.error(`Error creating location: ${error instanceof Error ? error.message : 'Unknown error'}`);
    },
});

return {
    createLocation: mutation.mutate,
    isPending: mutation.isPending,
    error: mutation.error, 
};
}