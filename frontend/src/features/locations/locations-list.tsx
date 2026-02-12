"use client";


import { useState } from "react";
import { Spinner } from "@/shared/components/ui/spinner";
import { Button } from "@/shared/components/ui/button";
import { CreateLocationDialog } from "./create-location-dialog";
import { useLocationsList } from "./model/use-locations-list";
import { useCreateLocation } from "./model/use-create-location";
import { Location } from "@/entities/locations/types";
import { UpdateLocationDialog } from "./update-location-dialog";
import { AlertCircleIcon, MapPinIcon, PlusIcon } from "lucide-react";
import { useGetLocationsFilter } from "./model/location-filters-store";
import { LocationsFilter } from "./locations-filter";
import { useGetGlobalSearch } from "@/shared/stored/global-search-store";
import LocationCard from "@/widgets/locations/location-card";

export default function LocationsList() {
  const { search, isActive, pageSize } = useGetLocationsFilter();
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
    search: search == "" ? globalSearch.search : search,
    isActive,
    pageSize,
  });

  const { error: createError } = useCreateLocation();

  return (
    <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  {/* Header Section */}
  <header className="mb-8">
    <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
      <div>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Locations
        </h1>
        <p className="text-gray-400 mt-2">
          Manage all your locations in one place
        </p>
      </div>

      <div className="flex items-center gap-3">
        <LocationsFilter />
        
        <Button
          onClick={() => setCreateOpen(true)}
          disabled={isPending}
          className="bg-linear-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white shadow-lg hover:shadow-xl transition-all duration-200"
          size="default"
        >
          <PlusIcon className="h-5 w-5" />
          Add
        </Button>
      </div>
    </div>
  </header>

  {/* Error Messages */}
  {isError && (
    <div className="mb-6 p-4 bg-red-900/20 border border-red-800 rounded-lg">
      <div className="flex items-center">
        <AlertCircleIcon className="h-5 w-5 text-red-400 mr-3" />
        <div>
          <p className="text-red-300 font-medium">Error loading locations</p>
          <p className="text-red-400 text-sm mt-1">
            {error?.message ?? "Unable to fetch locations. Please try again."}
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
