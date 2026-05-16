import { create } from "zustand";
import { useShallow } from "zustand/shallow";
import { createJSONStorage, persist } from "zustand/middleware";


export type LocationsFilterState = {
  departmentsIds: string[];
  search: string;
  isActive?: boolean;
  pageSize: number;
};

export type LocationDictionaryState = {
  search?: string;
  locationsIds?: string[];
  pageSize: number;
};

type Actions = {
  setSearch: (input: LocationsFilterState["search"]) => void;
  setDepartmentsIds: (departmentsIds: LocationsFilterState["departmentsIds"]) => void;
  setIsActive: (isActive: LocationsFilterState["isActive"]) => void;
};

type LocationsFilterStore = LocationsFilterState & Actions;

const initialState: LocationsFilterState = {
  search: "",
  departmentsIds: [],
  isActive: undefined,
  pageSize: 20,
};


const useLocationsFilterStore = create<LocationsFilterStore>()(
  persist(
    (set) => ({
      ...initialState,
       setDepartmentsIds: (ids: LocationsFilterState["departmentsIds"]) => 
              set(() => ({departmentsIds: ids})),
      setSearch: (input: LocationsFilterState["search"]) =>
        set(() => ({ search: input.trim() || "" })),
      setIsActive: (isActive: LocationsFilterState["isActive"]) =>
        set(() => ({ isActive })),
    }),
    {
      name: "locations-filters",
      storage: createJSONStorage(() => localStorage),
    },
  ),
);

export const useGetLocationsFilter = () => {
  return useLocationsFilterStore(
    useShallow((state) => ({
      departmentsIds: state.departmentsIds,
      search: state.search,
      isActive: state.isActive,
      pageSize: state.pageSize,
    })),
  );
};

export const setFilterSearch = (input: LocationsFilterState["search"]) =>
  useLocationsFilterStore.getState().setSearch(input);

export const setFilterLocationsDepartmentIds = (
  ids: LocationsFilterState["departmentsIds"],
) => {
  useLocationsFilterStore.getState().setDepartmentsIds(ids);
};

export const setFilterIsActive = (input: LocationsFilterState["isActive"]) =>
  useLocationsFilterStore.getState().setIsActive(input);
