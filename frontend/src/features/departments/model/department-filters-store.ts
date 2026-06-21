import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import { useShallow } from "zustand/shallow";


export type DepartmentsFilterState = {
  locationsIds: string[];
  search: string;
  isActive?: boolean;
  pageSize: number;
  parentId?: string | null;
  sortBy: string;
  sortDirection: string;
};

export type DepartmentDictionaryState = {
  search?: string;
  departmentsIds?: string[];
  pageSize: number;
};

type Actions = {
  setSearch: (input: DepartmentsFilterState["search"]) => void;
  setLocationsIds: (locationsIds: DepartmentsFilterState["locationsIds"]) => void;
  setParentId: (parentId: DepartmentsFilterState["parentId"]) => void;
  setIsActive: (isActive: DepartmentsFilterState["isActive"]) => void;
  setSortBy: (sortBy: DepartmentsFilterState["sortBy"]) => void;
  setSortDirection: (sortDirection: DepartmentsFilterState["sortDirection"]) => void;
};

type DepartmentsFilterStore = DepartmentsFilterState & Actions;

const initialState: DepartmentsFilterState = {
  search: "",
  locationsIds: [],
  parentId: null,
  isActive: undefined,
  pageSize: 20,
  sortBy: "name",
  sortDirection : "asc"
};

const useDepartmentsFilterStore = create<DepartmentsFilterStore>()(
  persist(
    (set) => ({
      ...initialState,
      setLocationsIds: (ids: DepartmentsFilterState["locationsIds"]) => 
        set(() => ({locationsIds: ids})),
      setParentId: (parentId: DepartmentsFilterState["parentId"]) => 
        set(() => ({parentId: parentId?.trim() || null})),
      setSearch: (input: DepartmentsFilterState["search"]) =>
        set(() => ({ search: input.trim() || "" })),
      setIsActive: (isActive: DepartmentsFilterState["isActive"]) =>
        set(() => ({ isActive })),
      setSortBy: (sortBy: DepartmentsFilterState["sortBy"]) =>
        set(() => ({sortBy: sortBy.trim() || ""})),
      setSortDirection: (sortDirection: DepartmentsFilterState["sortDirection"]) =>
        set(() => ({sortDirection: sortDirection.trim() || ""}))
    }),
    {
      name: "departments-filters",
      storage: createJSONStorage(() => localStorage),
    },
  ),
);

export const useGetDepartmentsFilter = () => {
  return useDepartmentsFilterStore(
    useShallow((state) => ({
      locationsIds: state.locationsIds,
      parentId: state.parentId,
      search: state.search,
      isActive: state.isActive,
      pageSize: state.pageSize,
      sortBy: state.sortBy,
      sortDirection: state.sortDirection
    })),
  );
};

export const setFilterSearch = (input: DepartmentsFilterState["search"]) =>
  useDepartmentsFilterStore.getState().setSearch(input);

export const setFilterDepartmentLocationsIds = (
  ids: DepartmentsFilterState["locationsIds"],
) => {
  useDepartmentsFilterStore.getState().setLocationsIds(ids);
};

export const setFilterIsActive = (input: DepartmentsFilterState["isActive"]) =>
  useDepartmentsFilterStore.getState().setIsActive(input);

export const setFilterParentId = (input: DepartmentsFilterState["parentId"]) =>
  useDepartmentsFilterStore.getState().setParentId(input);

export const setFilterSortBy = (input: DepartmentsFilterState["sortBy"]) =>
  useDepartmentsFilterStore.getState().setSortBy(input);

export const setFilterSortDirection = (input: DepartmentsFilterState["sortDirection"]) =>
  useDepartmentsFilterStore.getState().setSortDirection(input);
