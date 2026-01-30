"use client";

import { Position } from "@/entities/positions/types";
import { useGetGlobalSearch } from "@/shared/stored/global-search-store";
import { useGetPositionsFilter } from "./model/position-filters-store";
import { usePositionsList } from "./model/use-positions-list";
import { useCreatePosition } from "./model/use-create-position";
import { Button } from "@/shared/components/ui/button";
import { AlertCircleIcon, MapPinIcon, PlusIcon } from "lucide-react";
import { Spinner } from "@/shared/components/ui/spinner";
import { PositionsFilter } from "./positions-filters";
import PositionCard from "./position-card";
import { useState } from "react";

export default function PositionsList() {
  const { search, isActive, pageSize } = useGetPositionsFilter();
  const globalSearch = useGetGlobalSearch();

  const [createOpen, setCreateOpen] = useState(false);
  const [updateOpen, setUpdateOpen] = useState(false);
  const [selectedPosition, setSelectedPosition] = useState<Position | null>(
    null,
  );

  const {
      positions = [],
      isPending,
      error,
      isError,
      cursorRef,
      isFetchingNextPage,
    } = usePositionsList({
      search: search == "" ? globalSearch.search : search,
      isActive,
      pageSize,
    });
  
    const { error: createError } = useCreatePosition();

    return(
         <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  {/* Header Section */}
  <header className="mb-8">
    <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
      <div>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Positions
        </h1>
        <p className="text-gray-400 mt-2">
          Manage all your positions in one place
        </p>
      </div>

      <div className="flex items-center gap-3">
        <PositionsFilter />
        
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
          <p className="text-red-300 font-medium">Error loading positions</p>
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

  {/* Positions Grid */}
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
    ) : positions.length === 0 && !isError ? (
      <div className="text-center py-16 px-4 border-2 border-dashed border-gray-700 rounded-2xl bg-gray-900/50">
        <div className="max-w-md mx-auto">
          <MapPinIcon className="h-16 w-16 text-gray-600 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-white mb-2">
            No positions found
          </h3>
          <p className="text-gray-400 mb-6">
            Get started by adding your first positions
          </p>
          <Button
            onClick={() => setCreateOpen(true)}
            className="bg-linear-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800"
          >
            <PlusIcon className="h-5 w-5 mr-2" />
            Add First Position
          </Button>
        </div>
      </div>
    ) : (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 3xl:grid-cols-5 gap-6">
        {positions.map((position) => (
          <PositionCard
            key={position.id}
            position={position}
            onEdit={() => {
              setSelectedPosition(position);
              setUpdateOpen(true);
            }}
          />
        ))}
      </div>
    )}
  </section>

  {/* Dialogs */}
  {/* <CreatePositionDialog 
    open={createOpen} 
    onOpenChange={setCreateOpen} 
  />

  {selectedPosition && (
    <UpdatePositionDialog
      key={selectedPosition.id}
      location={selectedPosition}
      open={updateOpen}
      onOpenChange={setUpdateOpen}
    />
  )} */}

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
