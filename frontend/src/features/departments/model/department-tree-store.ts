import { DepartmentTreeItem } from "@/entities/departments/types";
import { create } from "zustand";
import { persist } from "zustand/middleware";
import { useShallow } from "zustand/react/shallow";

export const ROOT_PARENT_KEY = "__root__";
const EMPTY_DEPARTMENT_CHILDREN: DepartmentTreeItem[] = [];

export type DepartmentTreeBranchState = {
  page: number;
  totalPages: number;
  hasNextPage: boolean;
};

export type DepartmentsTreeState = {
  childrenByParentId: Record<string, DepartmentTreeItem[]>;
  expandedIds: string[];
  selectedId: string | null;
  branchesByParentId: Record<string, DepartmentTreeBranchState>;
};

type Actions = {
  setChildren: (
    parentId: string | null,
    children: DepartmentTreeItem[],
    branch?: DepartmentTreeBranchState,
  ) => void;
  appendChildren: (
    parentId: string,
    children: DepartmentTreeItem[],
    branch?: DepartmentTreeBranchState,
  ) => void;
  setExpandedIds: (expandedIds: string[]) => void;
  setSelectedId: (selectedId: string | null) => void;
};

type DepartmentsTreeStore = DepartmentsTreeState & Actions;

const initialState: DepartmentsTreeState = {
  childrenByParentId: {},
  expandedIds: [],
  selectedId: null,
  branchesByParentId: {},
};

const getParentKey = (parentId: string | null) => parentId ?? ROOT_PARENT_KEY;

const mergeChildrenById = (
  current: DepartmentTreeItem[],
  incoming: DepartmentTreeItem[],
) => {
  const childrenById = new Map<string, DepartmentTreeItem>();

  for (const child of current) {
    childrenById.set(child.id, child);
  }

  for (const child of incoming) {
    childrenById.set(child.id, child);
  }

  return Array.from(childrenById.values());
};

const useDepartmentsTreeStore = create<DepartmentsTreeStore>()(
  persist(
    (set) => ({
      ...initialState,
      setChildren: (parentId, children, branch) =>
        set((state) => {
          const parentKey = getParentKey(parentId);

          return {
            childrenByParentId: {
              ...state.childrenByParentId,
              [parentKey]: children,
            },
            branchesByParentId: branch
              ? {
                  ...state.branchesByParentId,
                  [parentKey]: branch,
                }
              : state.branchesByParentId,
          };
        }),
      appendChildren: (parentId, children, branch) =>
        set((state) => {
          const currentChildren = state.childrenByParentId[parentId] ?? [];

          return {
            childrenByParentId: {
              ...state.childrenByParentId,
              [parentId]: mergeChildrenById(currentChildren, children),
            },
            branchesByParentId: branch
              ? {
                  ...state.branchesByParentId,
                  [parentId]: branch,
                }
              : state.branchesByParentId,
          };
        }),
      setExpandedIds: (expandedIds) => set({ expandedIds }),
      setSelectedId: (selectedId) => set({ selectedId }),
    }),
    {
      name: "ds-departments-tree",
      partialize: (state) => ({
        expandedIds: state.expandedIds,
        selectedId: state.selectedId,
      }),
    },
  ),
);

export const useDepartmentsTreeState = () => {
  return useDepartmentsTreeStore(
    useShallow((state) => ({
      childrenByParentId: state.childrenByParentId,
      expandedIds: state.expandedIds,
      selectedId: state.selectedId,
      branchesByParentId: state.branchesByParentId,
    })),
  );
};

export const useDepartmentTreeBranch = (parentId: string | null) => {
  const parentKey = getParentKey(parentId);

  return useDepartmentsTreeStore(
    useShallow((state) => ({
      children: state.childrenByParentId[parentKey] ?? EMPTY_DEPARTMENT_CHILDREN,
      branch: state.branchesByParentId[parentKey],
      selectedId: state.selectedId,
    })),
  );
};

export const setDepartmentTreeChildren = (
  parentId: string | null,
  children: DepartmentTreeItem[],
  branch?: DepartmentTreeBranchState,
) => {
  useDepartmentsTreeStore.getState().setChildren(parentId, children, branch);
};

export const appendDepartmentTreeChildren = (
  parentId: string,
  children: DepartmentTreeItem[],
  branch?: DepartmentTreeBranchState,
) => {
  useDepartmentsTreeStore.getState().appendChildren(parentId, children, branch);
};

export const setDepartmentsTreeExpandedIds = (expandedIds: string[]) => {
  useDepartmentsTreeStore.getState().setExpandedIds(expandedIds);
};

export const setSelectedDepartmentId = (selectedId: string | null) => {
  useDepartmentsTreeStore.getState().setSelectedId(selectedId);
};

export const getDepartmentTreeSnapshot = () => useDepartmentsTreeStore.getState();
