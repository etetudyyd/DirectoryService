import { create } from "zustand";
import { useShallow } from "zustand/shallow";
import { createJSONStorage, persist } from "zustand/middleware";

export type DepartmentsFilterState = {
  search: string;
  isActive?: boolean;
  pageSize: number;
};

export type DepartmentDictionaryState = {
  search?: string;
  departmentsIds?: string[];
  pageSize: number;
};

type Actions = {
  setSearch: (input: DepartmentsFilterState["search"]) => void;
  setIsActive: (isActive: DepartmentsFilterState["isActive"]) => void;
};

type DepartmentsFilterStore = DepartmentsFilterState & Actions;

const initialState: DepartmentsFilterState = {
  search: "",
  isActive: undefined,
  pageSize: 20,
};

const useDepartmentsFilterStore = create<DepartmentsFilterStore>()(
  persist(
    (set) => ({
      ...initialState,
      setSearch: (input: DepartmentsFilterState["search"]) =>
        set(() => ({ search: input.trim() || "" })),
      setIsActive: (isActive: DepartmentsFilterState["isActive"]) =>
        set(() => ({ isActive })),
    }),
    {
      name: "locations-filters",
      storage: createJSONStorage(() => localStorage),
    },
  ),
);

export const useGetDepartmentsFilter = () => {
  return useDepartmentsFilterStore(
    useShallow((state) => ({
      search: state.search,
      isActive: state.isActive,
      pageSize: state.pageSize,
    })),
  );
};

export const setFilterSearch = (input: DepartmentsFilterState["search"]) =>
  useDepartmentsFilterStore.getState().setSearch(input);

export const setFilterIsActive = (input: DepartmentsFilterState["isActive"]) =>
  useDepartmentsFilterStore.getState().setIsActive(input);
