import { create } from "zustand";
import { useShallow } from "zustand/shallow";
import { createJSONStorage, persist } from "zustand/middleware";

export type PositionsFilterState = {
  departmentsIds: string[];
  search: string;
  isActive?: boolean;
  pageSize: number;
};

type Actions = {
  setSearch: (input: PositionsFilterState["search"]) => void;
  setDepartmentsIds: (departmentsIds: PositionsFilterState["departmentsIds"]) => void;
  setIsActive: (isActive: PositionsFilterState["isActive"]) => void;
};

type PositionsFilterStore = PositionsFilterState & Actions;

const initialState: PositionsFilterState = {
  search: "",
  departmentsIds: [],
  isActive: undefined,
  pageSize: 20,
};

const usePositionsFilterStore = create<PositionsFilterStore>()(
  persist(
    (set) => ({
      ...initialState,
      setDepartmentsIds: (ids: PositionsFilterState["departmentsIds"]) => 
        set(() => ({departmentsIds: ids})),
      setSearch: (input: PositionsFilterState["search"]) =>
        set(() => ({ search: input.trim() || "" })),
      setIsActive: (isActive: PositionsFilterState["isActive"]) =>
        set(() => ({ isActive })),
    }),
    {
      name: "positions-filters",
      storage: createJSONStorage(() => localStorage),
    },
  ),
);

export const useGetPositionsFilter = () => {
  return usePositionsFilterStore(
    useShallow((state) => ({
      departmentsIds: state.departmentsIds,
      search: state.search,
      isActive: state.isActive,
      pageSize: state.pageSize,
    })),
  );
};

export const setFilterSearch = (input: PositionsFilterState["search"]) =>
  usePositionsFilterStore.getState().setSearch(input);

export const setFilterPositionsDepartmentIds = (
  ids: PositionsFilterState["departmentsIds"],
) => {
  usePositionsFilterStore.getState().setDepartmentsIds(ids);
};

export const setFilterIsActive = (input: PositionsFilterState["isActive"]) =>
  usePositionsFilterStore.getState().setIsActive(input);
