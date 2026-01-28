import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select";
import {
  setFilterIsActive,
  setFilterSearch as setFilterSearch,
  useGetLocationsFilter,
} from "./model/location-filters-store";
import { Input } from "@/shared/components/ui/input";
import { Search } from "lucide-react";

export function LocationsFilter() {
  const { search, isActive } = useGetLocationsFilter();


  return (
    <div className="flex items-center gap-2 min-w-100 max-w-100">
      <div className="relative flex-3">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
        <Input
          value={search}
          onChange={(e) => setFilterSearch(e.target.value)}
          placeholder="Search by name..."
          className="pl-9"
        />
      </div>

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
        <SelectTrigger className="flex-1 w-auto">
          <SelectValue placeholder="Filter" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">All</SelectItem>
          <SelectItem value="active">Active</SelectItem>
          <SelectItem value="inactive">Inactive</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}
