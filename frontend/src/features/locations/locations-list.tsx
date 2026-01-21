"use client";

import { Input } from "@/shared/components/ui/input";
import LocationCard from "@/features/locations/location-card";
import { RefCallback, useCallback, useState } from "react";
import { Spinner } from "@/shared/components/ui/spinner";
import { Button } from "@/shared/components/ui/button";
import { CreateLocationDialog } from "./create-location-dialog";
import { useLocationsList } from "./model/use-locations-list";
import { useCreateLocation } from "./model/use-create-location";

export default function LocationsList() {
  const [open, setOpen] = useState(false);

  const {
    locations,
    totalItems,
    isLoading,
    error,
    isError,
    cursorRef,
    isFetchingNextPage,
  } = useLocationsList();

  const { isPending, error: createError } = useCreateLocation();

  if (isLoading) {
    return (
      <div className="flex justify-center container mx-auto py-8 px-4">
        <Spinner />
      </div>
    );
  }

  if (isError) {
    return <div>Error: {error ? error.message : "Undefined error"}</div>;
  }

  if (!locations || totalItems === 0) {
    return (
      <div className="text-center mt-20 text-gray-400">No locations found.</div>
    );
  }

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
          <Input
            placeholder="Поиск по названию или адресу"
            className="min-w-65 bg-slate-800/50"
          />
          <Button
            onClick={() => setOpen(true)}
            disabled={isPending}
            variant="default"
          >
            Добавить локацию
          </Button>
          {createError && (
            <div className="text-red-500 text-sm">{createError.message}</div>
          )}
        </div>
      </div>

      <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {locations.map((location) => (
          <LocationCard key={location.id} location={location} />
        ))}
      </section>

      {/* {totalPages && (
        <LocationsPagination
          page={page}
          totalPages={totalPages}
          onPageChange={(p) => setPage(p)}
        />
      )} */}

      <CreateLocationDialog open={open} onOpenChange={setOpen} />

      <div ref={cursorRef} className="flex justify-center py-4">
        {isFetchingNextPage && <Spinner />}
      </div>
    </main>
  );
}
