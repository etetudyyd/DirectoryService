"use client";

import { Spinner } from "@/shared/components/ui/spinner";
import { useDepartmentRoots } from "../model/use-department-roots";
import { Button } from "@/shared/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger, TooltipProvider } from "@/shared/components/ui/tooltip";
import { setDepartmentsTreeExpandedNodes, useDepartmentsExpandedNodes } from "../model/department-tree-store";
import { TreeExpander, TreeIcon, TreeLabel, TreeNode, TreeNodeContent, TreeNodeTrigger, TreeProvider, TreeView } from "@/shared/components/ui/tree";



export default function DepartmentTree() {
  const { expandedNodes } = useDepartmentsExpandedNodes();
  const {
    departments: rootDepartments,
    isPending,
    isError,
    error,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
  } = useDepartmentRoots();

  if (isPending) {
    return (
      <div className="flex justify-center items-center py-4">
        <Spinner />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="bg-red-50 border-l-4 border-red-500 p-4 rounded text-red-800">
        <p className="font-semibold">Ошибка загрузки корневых подразделений</p>
        <p className="text-sm mt-1">{error?.message}</p>
      </div>
    );
  }

  if (!rootDepartments || rootDepartments.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="text-slate-400 text-center">
          <p className="text-lg font-medium mb-2">
            Departments not found
          </p>
          <p className="text-sm">
            There are no departments to display.
          </p>
        </div>
      </div>
    );
  }

  if (!isPending && rootDepartments && rootDepartments.length > 0) {
    return (
      <></>
    );
  }
}