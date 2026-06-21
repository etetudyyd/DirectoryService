import { departmentsQueryOptions } from "@/entities/departments/api";
import { DepartmentDetails } from "@/entities/departments/types";
import { useQuery } from "@tanstack/react-query";

export function useGetDepartment(departmentId: string) {
  const { 
    data,
    isPending,
    error,
    isError,
    refetch } = useQuery({
    ...departmentsQueryOptions.getDepartmentOptions(departmentId),
    select: (response: any): DepartmentDetails | undefined => {
      return response?.department;
    }
  });

  return {
    department: data,
    isPending,
    error,
    isError,
    refetch,
  };
}