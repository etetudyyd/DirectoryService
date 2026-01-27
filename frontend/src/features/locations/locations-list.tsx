"use client";

import LocationCard from "@/features/locations/location-card";
import { useState } from "react";
import { Spinner } from "@/shared/components/ui/spinner";
import { Button } from "@/shared/components/ui/button";
import { CreateLocationDialog } from "./create-location-dialog";
import { useLocationsList } from "./model/use-locations-list";
import { useCreateLocation } from "./model/use-create-location";
import { Location } from "@/entities/locations/types";
import { UpdateLocationDialog } from "./update-location-dialog";
import { PlusIcon } from "lucide-react";
import { useGetLocationsFilter } from "./model/location-filters-store";
import { LocationsFilter } from "./locations-filter";

export default function LocationsList() {
  const { search, isActive, pageSize } = useGetLocationsFilter();

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
    search,
    isActive,
    pageSize,
  });

  const { error: createError } = useCreateLocation();

  return (
    <main className="max-w-6xl mx-auto p-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
        <div>
          <h1 className="text-2xl font-semibold text-white">Локации</h1>
          <p className="text-sm text-slate-400 mt-1">
            Список всех локаций организации
          </p>
        </div>

        <div className="flex items-center gap-3">
          <LocationsFilter />

          <Button
            onClick={() => setCreateOpen(true)}
            disabled={isPending}
            className="hover:bg-gray-400"
          >
            <PlusIcon className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {isError && (
        <div className="text-red-500 mb-4">
          Error: {error?.message ?? "Undefined error"}
        </div>
      )}

      {createError && (
        <div className="text-red-500 mb-4">{createError.message}</div>
      )}

      <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {isPending ? (
          <div className="col-span-full flex justify-center py-10">
            <Spinner />
          </div>
        ) : locations.length === 0 && !isError ? (
          <div className="col-span-full text-center text-gray-400 py-10">
            No locations found.
          </div>
        ) : (
          locations.map((location) => (
            <LocationCard
              key={location.id}
              location={location}
              onEdit={() => {
                setSelectedLocation(location);
                setUpdateOpen(true);
              }}
            />
          ))
        )}
      </section>

      <CreateLocationDialog open={createOpen} onOpenChange={setCreateOpen} />

      {selectedLocation && (
        <UpdateLocationDialog
          key={selectedLocation.id}
          location={selectedLocation}
          open={updateOpen}
          onOpenChange={setUpdateOpen}
        />
      )}

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner />}
      </div>
    </main>
  );
}
