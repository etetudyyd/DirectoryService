import { Search, Filter, X } from "lucide-react";
import {
  setFilterIsActive,
  setFilterPositionsDepartmentIds,
  setFilterSearch,
  useGetPositionsFilter,
} from "./model/position-filters-store";
import { Input } from "@/shared/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";
import DepartmentsSelectFilter from "../departments/departments-select-filter";
import { Button } from "@/shared/components/ui/button";
import { Badge } from "@/shared/components/ui/badge";

export function PositionsFilter() {
  const { departmentsIds, search, isActive } = useGetPositionsFilter();

  const hasActiveFilters = search || departmentsIds?.length || isActive !== undefined;

  return (
    <div className="w-full space-y-4">
      {/* Main Filters Row */}
      <div className="flex flex-col lg:flex-row gap-3">
        {/* Search Input */}
        <div className="relative flex-1 min-w-0">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            value={search}
            onChange={(e) => setFilterSearch(e.target.value)}
            placeholder="Search positions..."
            className="pl-9 h-10"
          />
        </div>

        {/* Departments Filter */}
        <div className="flex-1 min-w-62.5">
          <DepartmentsSelectFilter
            selectedDepartmentIds={departmentsIds}
            onDepartmentChange={setFilterPositionsDepartmentIds}
          />
        </div>

        {/* Status Filter */}
        <div className="w-37.5">
          <Select
            value={
              isActive === undefined ? "all" : isActive ? "active" : "inactive"
            }
            onValueChange={(value) => {
              if (value === "all") setFilterIsActive(undefined);
              else if (value === "active") setFilterIsActive(true);
              else setFilterIsActive(false);
            }}
          >
            <SelectTrigger className="h-10">
              <div className="flex items-center gap-2">
                <Filter className="h-4 w-4 text-muted-foreground" />
                <SelectValue placeholder="Status" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">
                <div className="flex items-center gap-2">
                  <div className="h-2 w-2 rounded-full bg-gray-400" />
                  All statuses
                </div>
              </SelectItem>
              <SelectItem value="active">
                <div className="flex items-center gap-2">
                  <div className="h-2 w-2 rounded-full bg-emerald-500" />
                  Active
                </div>
              </SelectItem>
              <SelectItem value="inactive">
                <div className="flex items-center gap-2">
                  <div className="h-2 w-2 rounded-full bg-amber-500" />
                  Inactive
                </div>
              </SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Clear All Button */}
        {hasActiveFilters && (
          <Button
            variant="outline"
            size="sm"
            onClick={() => {
              setFilterSearch("");
              setFilterPositionsDepartmentIds([]);
              setFilterIsActive(undefined);
            }}
            className="h-10 shrink-0"
          >
            <X className="h-4 w-4 mr-2" />
            Clear all
          </Button>
        )}
      </div>

      {/* Active Filters Badges */}
      {hasActiveFilters && (
        <div className="flex flex-wrap items-center gap-2 pt-3 border-t">
          <span className="text-sm text-muted-foreground">Active filters:</span>
          
          {search && (
            <Badge variant="secondary" className="gap-1">
              Search: "{search}"
              <button
                onClick={() => setFilterSearch("")}
                className="ml-1 hover:bg-secondary/80 rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          )}
          
          {departmentsIds?.map(id => (
            <Badge key={id} variant="secondary" className="gap-1">
              Dept: {id.substring(0, 8)}...
              <button
                onClick={() => {
                  const newIds = departmentsIds.filter(dId => dId !== id);
                  setFilterPositionsDepartmentIds(newIds);
                }}
                className="ml-1 hover:bg-secondary/80 rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          ))}
          
          {isActive !== undefined && (
            <Badge variant="secondary" className="gap-1">
              Status: {isActive ? 'Active' : 'Inactive'}
              <button
                onClick={() => setFilterIsActive(undefined)}
                className="ml-1 hover:bg-secondary/80 rounded-full p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          )}
        </div>
      )}
    </div>
  );
}