"use client";


import { useState } from "react";
import { Spinner } from "@/shared/components/ui/spinner";
import { Button } from "@/shared/components/ui/button";
import { CreateLocationDialog } from "./create-location-dialog";
import { useLocationsList } from "./model/use-locations-list";
import { useCreateLocation } from "./model/use-create-location";
import { Location } from "@/entities/locations/types";
import { UpdateLocationDialog } from "./update-location-dialog";
import { AlertCircleIcon, MapPinIcon, PlusIcon, Search, X } from "lucide-react";
import { setFilterIsActive, setFilterLocationsDepartmentIds, setFilterSearch, useGetLocationsFilter } from "./model/location-filters-store";
import { LocationsFilter } from "./locations-filters";
import { useGetGlobalSearch } from "@/shared/stored/global-search-store";
import LocationCard from "@/widgets/locations/location-card";
import { Input } from "@/shared/components/ui/input";
import DepartmentItemSelector from "../../widgets/departments/department-item-selector";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/shared/components/ui/select";
import { Badge } from "@/shared/components/ui/badge";

export default function LocationsList() {
  const {departmentsIds, search, isActive, pageSize } = 
    useGetLocationsFilter();
  const globalSearch = useGetGlobalSearch();

  const [createOpen, setCreateOpen] = useState(false);
  const [updateOpen, setUpdateOpen] = useState(false);
  const [selectedLocation, setSelectedLocation] = useState<Location | null>(
    null,
  );

  const {
    locations = [],
    isPending,
    error,
    isError,
    cursorRef,
    isFetchingNextPage,
  } = useLocationsList({
    departmentsIds,
    search: search == "" ? globalSearch.search : search,
    isActive,
    pageSize,
  });

  const { error: createError } = useCreateLocation();

  return (
  <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header Section */}
      <div className="flex flex-col lg:flex-row gap-6 lg:items-center lg:justify-between mb-8">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight text-white">
            Locations
          </h1>
          <p className="text-gray-400">
            Manage all your locations in one place
          </p>
        </div>

        <Button
          onClick={() => setCreateOpen(true)}
          disabled={isPending}
          className="bg-linear-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white shadow-lg hover:shadow-xl transition-all duration-200 shrink-0"
          size="default"
        >
          <PlusIcon className="h-5 w-5 mr-2" />
          Add Location
        </Button>
      </div>

      {/* Filters Section */}
      <div className="bg-gray-900/50 rounded-xl p-6 mb-8 border border-gray-800">
        <div className="space-y-4">
          <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-3 w-full">
            <div className="flex-1 min-w-0">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  value={search}
                  onChange={(e) => setFilterSearch(e.target.value)}
                  placeholder="Search positions..."
                  className="pl-9 h-11 bg-gray-800 border-gray-700 text-white placeholder:text-gray-500 w-full"
                />
              </div>
            </div>

            {/* Остальные фильтры в ряд */}
            <div className="flex flex-col sm:flex-row gap-3 sm:items-center shrink-0">
              {/* Departments Filter */}
              <div className="sm:w-68">
                <DepartmentItemSelector
                  selectedItemsIds={departmentsIds}
                  onDepartmentChange={setFilterLocationsDepartmentIds}
                />
              </div>

              {/* Status Filter */}
              <div className="sm:w-24">
                <Select
                  value={
                    isActive === undefined
                      ? "all"
                      : isActive
                        ? "active"
                        : "inactive"
                  }
                  onValueChange={(value) => {
                    if (value === "all") setFilterIsActive(undefined);
                    else if (value === "active") setFilterIsActive(true);
                    else setFilterIsActive(false);
                  }}
                >
                  <SelectTrigger className="h-11 bg-gray-800 border-gray-700 text-white w-full">
                    <SelectValue placeholder="Status" />
                  </SelectTrigger>
                  <SelectContent className="bg-gray-900 border-gray-800">
                    <DepartmentItemSelector value="all" className="hover:bg-gray-800">
                      All
                    </DepartmentItemSelector>
                    <DepartmentItemSelector value="active" className="hover:bg-gray-800">
                      Active
                    </DepartmentItemSelector>
                    <DepartmentItemSelector value="inactive" className="hover:bg-gray-800">
                      Inactive
                    </DepartmentItemSelector>
                  </SelectContent>
                </Select>
              </div>
            </div>
          </div>

          {/* Active Filters Badges */}
          {(search || departmentsIds?.length || isActive !== undefined) && (
            <div className="flex flex-wrap items-center gap-2 pt-4 border-t border-gray-800">
              <span className="text-sm text-gray-400">Active filters:</span>

              {search && (
                <Badge
                  variant="secondary"
                  className="gap-1 bg-gray-800 text-gray-300 border-gray-700"
                >
                  Search: "{search}"
                  <button
                    onClick={() => setFilterSearch("")}
                    className="ml-1 hover:bg-gray-700 rounded-full p-0.5"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              )}

              {departmentsIds?.map((id) => (
                <Badge
                  key={id}
                  variant="secondary"
                  className="gap-1 bg-gray-800 text-gray-300 border-gray-700"
                >
                  Dept: {id.substring(0, 8)}...
                  <button
                    onClick={() => {
                      const newIds = departmentsIds.filter((dId) => dId !== id);
                      setFilterLocationsDepartmentIds(newIds);
                    }}
                    className="ml-1 hover:bg-gray-700 rounded-full p-0.5"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              ))}

              {isActive !== undefined && (
                <Badge
                  variant="secondary"
                  className="gap-1 bg-gray-800 text-gray-300 border-gray-700"
                >
                  Status: {isActive ? "Active" : "Inactive"}
                  <button
                    onClick={() => setFilterIsActive(undefined)}
                    className="ml-1 hover:bg-gray-700 rounded-full p-0.5"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              )}
            </div>
          )}
        </div>
      </div>
      {/* Error Messages */}
      {isError && (
        <div className="mb-6 p-4 bg-red-900/20 border border-red-800 rounded-lg">
          <div className="flex items-center">
            <AlertCircleIcon className="h-5 w-5 text-red-400 mr-3" />
            <div>
              <p className="text-red-300 font-medium">
                Error loading positions
              </p>
              <p className="text-red-400 text-sm mt-1">
                {error?.message ??
                  "Unable to fetch locations. Please try again."}
              </p>
            </div>
          </div>
        </div>
      )}

      {createError && (
        <div className="mb-6 p-4 bg-red-900/20 border border-red-800 rounded-lg">
          <div className="flex items-center">
            <AlertCircleIcon className="h-5 w-5 text-red-400 mr-3" />
            <p className="text-red-300">{createError.message}</p>
          </div>
        </div>
      )}

  {/* Locations Grid */}
  <section className="mb-10">
    {isPending ? (
      <div className="flex justify-center items-center min-h-100">
        <div className="text-center">
          <div className="inline-block">
            <Spinner className="h-10 w-10 text-blue-900" />
          </div>
          <p className="text-gray-400">Loading...</p>
        </div>
      </div>
    ) : locations.length === 0 && !isError ? (
      <div className="text-center py-16 px-4 border-2 border-dashed border-gray-700 rounded-2xl bg-gray-900/50">
        <div className="max-w-md mx-auto">
          <MapPinIcon className="h-16 w-16 text-gray-600 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-white mb-2">
            No locations found
          </h3>
          <p className="text-gray-400 mb-6">
            Get started by adding your first location
          </p>
          <Button
            onClick={() => setCreateOpen(true)}
            className="bg-linear-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800"
          >
            <PlusIcon className="h-5 w-5 mr-2" />
            Add First Location
          </Button>
        </div>
      </div>
    ) : (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 3xl:grid-cols-5 gap-6">
        {locations.map((location) => (
          <LocationCard
            key={location.id}
            location={location}
            onEdit={() => {
              setSelectedLocation(location);
              setUpdateOpen(true);
            }}
          />
        ))}
      </div>
    )}
  </section>

  {/* Dialogs */}
  <CreateLocationDialog 
    open={createOpen} 
    onOpenChange={setCreateOpen} 
  />

  {selectedLocation && (
    <UpdateLocationDialog
      key={selectedLocation.id}
      location={selectedLocation}
      open={updateOpen}
      onOpenChange={setUpdateOpen}
    />
  )}

  {/* Infinite Scroll Loader */}
  <div ref={cursorRef} className="py-6">
    {isFetchingNextPage && (
      <div className="flex justify-center">
        <div className="inline-flex items-center gap-3 px-6 py-3 bg-gray-800/50 rounded-full">
          <Spinner className="h-6 w-6 text-blue-900" />
        </div>
      </div>
    )}
  </div>
</main>
  );
}
