"use client";

import { useState } from "react";

import { departmentsApi } from "@/entities/departments/api";
import { DepartmentTreeItem } from "@/entities/departments/types";
import { PAGE_SIZE } from "@/shared/api/types";
import { Button } from "@/shared/components/ui/button";
import { Spinner } from "@/shared/components/ui/spinner";
import {
  TreeExpander,
  TreeIcon,
  TreeLabel,
  TreeNode,
  TreeNodeContent,
  TreeNodeTrigger,
} from "@/shared/components/ui/tree";

import {
  appendDepartmentTreeChildren,
  setDepartmentTreeChildren,
  useDepartmentTreeBranch,
} from "../model/department-tree-store";

const canExpandDepartment = (department: DepartmentTreeItem) =>
  department.hasChildren ?? department.hasMoreChildren;

type DepartmentTreeNodeProps = {
  department: DepartmentTreeItem;
  isLast?: boolean;
  onSelect: (departmentId: string) => void;
};

export default function DepartmentTreeNode({
  department,
  isLast = false,
  onSelect,
}: DepartmentTreeNodeProps) {
  const { children, branch } = useDepartmentTreeBranch(department.id);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasChildren = canExpandDepartment(department);
  const hasLoadedBranch = branch !== undefined;
  const canLoadMore = Boolean(branch?.hasNextPage);

  const loadChildren = async () => {
    if (!hasChildren || hasLoadedBranch || isLoading) {
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      const response = await departmentsApi.getChildrenDepartments({
        parentId: department.id,
        page: 1,
        pageSize: PAGE_SIZE,
      });

      if (!response) {
        throw new Error("Пустой ответ при загрузке дочерних подразделений");
      }

      setDepartmentTreeChildren(department.id, response.items, {
        page: response.page,
        totalPages: response.totalPages,
        hasNextPage: response.page < response.totalPages,
      });
    } catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : "Не удалось загрузить дочерние подразделения",
      );
    } finally {
      setIsLoading(false);
    }
  };

  const loadMoreChildren = async (event: React.MouseEvent) => {
    event.stopPropagation();

    if (!branch?.hasNextPage || isLoading) {
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      const response = await departmentsApi.getChildrenDepartments({
        parentId: department.id,
        page: branch.page + 1,
        pageSize: PAGE_SIZE,
      });

      if (!response) {
        throw new Error("Пустой ответ при загрузке дочерних подразделений");
      }

      appendDepartmentTreeChildren(department.id, response.items, {
        page: response.page,
        totalPages: response.totalPages,
        hasNextPage: response.page < response.totalPages,
      });
    } catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : "Не удалось загрузить следующую порцию подразделений",
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <TreeNode
      nodeId={department.id}
      level={department.depth}
      isLast={isLast}
    >
      <TreeNodeTrigger
        onClick={() => {
          onSelect(department.id);
          void loadChildren();
        }}
      >
        <TreeExpander
          hasChildren={hasChildren}
          onClick={() => {
            void loadChildren();
          }}
        />
        <TreeIcon hasChildren={hasChildren} />
        <TreeLabel>{department.name}</TreeLabel>
      </TreeNodeTrigger>

      {hasChildren && (
        <TreeNodeContent hasChildren={hasChildren}>
          {children.map((child, index) => (
            <DepartmentTreeNode
              key={child.id}
              department={child}
              isLast={index === children.length - 1 && !canLoadMore}
              onSelect={onSelect}
            />
          ))}

          {isLoading && (
            <div className="py-1 pl-6">
              <Spinner />
            </div>
          )}

          {errorMessage && (
            <div className="py-1 pl-6">
              <p className="text-sm text-red-400">{errorMessage}</p>
            </div>
          )}

          {canLoadMore && !isLoading && (
            <div className="py-1 pl-6">
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={loadMoreChildren}
              >
                Показать еще
              </Button>
            </div>
          )}
        </TreeNodeContent>
      )}
    </TreeNode>
  );
}
