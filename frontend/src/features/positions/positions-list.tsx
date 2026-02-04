"use client";

import { Position } from "@/entities/positions/types";
import { useGetGlobalSearch } from "@/shared/stored/global-search-store";
import { useGetPositionsFilter } from "./model/position-filters-store";
import { usePositionsList } from "./model/use-positions-list";
import { useCreatePosition } from "./model/use-create-position";
import { Button } from "@/shared/components/ui/button";
import { AlertCircleIcon, MapPinIcon, PlusIcon, Search, X } from "lucide-react";
import { Spinner } from "@/shared/components/ui/spinner";
import PositionCard from "../../widgets/positions/position-card";
import { useState } from "react";
import { CreatePositionDialog } from "./create-position-dialog";
import { Input } from "@/shared/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";
import { Badge } from "@/shared/components/ui/badge";
import DepartmentsSelectFilter from "../departments/departments-select-filter";
import {
  setFilterIsActive,
  setFilterPositionsDepartmentIds,
  setFilterSearch,
} from "./model/position-filters-store";

export default function PositionsList() {
  const { departmentsIds, search, isActive, pageSize } =
    useGetPositionsFilter();
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
    departmentsIds,
    search: search == "" ? globalSearch.search : search,
    isActive,
    pageSize,
  });

  const { error: createError } = useCreatePosition();

  return (
    <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header Section */}
      <div className="flex flex-col lg:flex-row gap-6 lg:items-center lg:justify-between mb-8">
        <div className="space-y-2">
          <h1 className="text-3xl font-bold tracking-tight text-white">
            Positions
          </h1>
          <p className="text-gray-400">
            Manage all your positions in one place
          </p>
        </div>

        <Button
          onClick={() => setCreateOpen(true)}
          disabled={isPending}
          className="bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 text-white shadow-lg hover:shadow-xl transition-all duration-200 shrink-0"
          size="default"
        >
          <PlusIcon className="h-5 w-5 mr-2" />
          Add Position
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
                <DepartmentsSelectFilter
                  selectedDepartmentIds={departmentsIds}
                  onDepartmentChange={setFilterPositionsDepartmentIds}
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
                    <SelectItem value="all" className="hover:bg-gray-800">
                      All
                    </SelectItem>
                    <SelectItem value="active" className="hover:bg-gray-800">
                      Active
                    </SelectItem>
                    <SelectItem value="inactive" className="hover:bg-gray-800">
                      Inactive
                    </SelectItem>
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
                      setFilterPositionsDepartmentIds(newIds);
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

      {/* Positions Grid */}
      <section className="mb-10">
        {isPending ? (
          <div className="flex flex-col justify-center items-center min-h-[400px]">
            <Spinner className="h-12 w-12 text-blue-900 mb-4" />
            <p className="text-gray-400">Loading positions...</p>
          </div>
        ) : positions.length === 0 && !isError ? (
          <div className="flex flex-col items-center justify-center py-16 px-4 border-2 border-dashed border-gray-700 rounded-2xl bg-gray-900/50">
            <MapPinIcon className="h-20 w-20 text-gray-600 mb-6" />
            <h3 className="text-2xl font-semibold text-white mb-3">
              No positions found
            </h3>
            <p className="text-gray-400 mb-6 text-center max-w-md">
              {search || departmentsIds?.length || isActive !== undefined
                ? "No positions match your current filters. Try adjusting your search criteria."
                : "Get started by adding your first position"}
            </p>
            <Button
              onClick={() => setCreateOpen(true)}
              className="bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800"
            >
              <PlusIcon className="h-5 w-5 mr-2" />
              {search || departmentsIds?.length || isActive !== undefined
                ? "Clear filters and add position"
                : "Add First Position"}
            </Button>
          </div>
        ) : (
          <>
            {/* Results Count */}
            <div className="flex items-center justify-between mb-6">
              <div className="text-gray-400 text-sm">
                Showing{" "}
                <span className="text-white font-semibold">
                  {positions.length}
                </span>{" "}
                position{positions.length !== 1 ? "s" : ""}
              </div>
            </div>

            {/* Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-6">
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
          </>
        )}
      </section>

      {/* Dialogs */}
      <CreatePositionDialog open={createOpen} onOpenChange={setCreateOpen} />

      {/* Infinite Scroll Loader */}
      <div ref={cursorRef} className="py-6">
        {isFetchingNextPage && (
          <div className="flex justify-center">
            <div className="inline-flex items-center gap-3 px-6 py-3 bg-gray-800/50 rounded-full">
              <Spinner className="h-6 w-6 text-blue-900" />
              <span className="text-gray-400">Loading more...</span>
            </div>
          </div>
        )}
      </div>
    </main>
  );
}
