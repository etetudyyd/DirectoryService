"use client";

import { useEffect } from "react";

import { Button } from "@/shared/components/ui/button";
import { Spinner } from "@/shared/components/ui/spinner";
import { TreeProvider, TreeView } from "@/shared/components/ui/tree";

import {
  setDepartmentTreeChildren,
  setDepartmentsTreeExpandedIds,
  setSelectedDepartmentId,
  useDepartmentTreeBranch,
  useDepartmentsTreeState,
} from "../model/department-tree-store";
import { useDepartmentRoots } from "../model/use-department-roots";
import DepartmentTreeNode from "./department-tree-node";

export default function DepartmentTree() {
  const {
    departments: rootDepartments,
    isPending,
    isError,
    error,
    hasNextPage,
    isFetchingNextPage,
    fetchNextPage,
    page,
    totalPages,
  } = useDepartmentRoots();

  const { children: rootChildren } = useDepartmentTreeBranch(null);
  const { expandedIds, selectedId } = useDepartmentsTreeState();

  useEffect(() => {
    if (!rootDepartments) {
      return;
    }

    setDepartmentTreeChildren(null, rootDepartments, {
      page: page ?? 1,
      totalPages: totalPages ?? 0,
      hasNextPage: Boolean(hasNextPage),
    });
  }, [hasNextPage, page, rootDepartments, totalPages]);

  if (isPending && rootChildren.length === 0) {
    return (
      <div className="flex items-center justify-center py-4">
        <Spinner />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="rounded border-l-4 border-red-500 bg-red-950/40 p-4 text-red-200">
        <p className="font-semibold">Не удалось загрузить корневые подразделения</p>
        <p className="mt-1 text-sm">{error?.message}</p>
      </div>
    );
  }

  if (rootChildren.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="text-center text-slate-400">
          <p className="mb-2 text-lg font-medium">Подразделения не найдены</p>
          <p className="text-sm">В дереве пока нет корневых узлов.</p>
        </div>
      </div>
    );
  }

  return (
    <TreeProvider
      defaultExpandedIds={expandedIds}
      selectedIds={selectedId ? [selectedId] : []}
      selectable={false}
      onExpandChange={setDepartmentsTreeExpandedIds}
      showLines
      showIcons
    >
      <TreeView className="p-0">
        {rootChildren.map((department, index) => (
          <DepartmentTreeNode
            key={department.id}
            department={department}
            isLast={index === rootChildren.length - 1}
            onSelect={setSelectedDepartmentId}
          />
        ))}
      </TreeView>

      {hasNextPage && (
        <div className="pt-2">
          <Button
            type="button"
            variant="ghost"
            size="sm"
            disabled={isFetchingNextPage}
            onClick={() => fetchNextPage()}
          >
            {isFetchingNextPage ? "Загрузка..." : "Показать еще"}
          </Button>
        </div>
      )}
    </TreeProvider>
  );
}
