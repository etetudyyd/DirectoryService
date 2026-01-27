import { Card } from "@/shared/components/ui/card";
import { Location } from "@/entities/locations/types";
import { Button } from "@/shared/components/ui/button";
import { Edit2Icon, Trash, Clock, Users, Calendar, Globe } from "lucide-react";
import { useDeleteLocation } from "./model/use-delete-location";

type Props = {
  location: Location;
  onEdit: () => void;
};

const formatedDateTime = (date: Date | string | null) => {
  if (!date) return "N/A";

  try {
    const d = new Date(date);
    if (isNaN(d.getTime())) return "N/A";

    return d
      .toLocaleString("en-GB", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        hour12: false,
      })
      .replace(",", "");
  } catch {
    return "N/A";
  }
};

export default function LocationCard({ location, onEdit }: Props) {
  console.log("CreatedAt:", {
    value: location.createdAt,
    type: typeof location.createdAt,
    isDate: location.createdAt instanceof Date,
  });

  const { deleteLocation, isPending } = useDeleteLocation();

  const handleDelete = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();

    if (confirm("Are you sure you want to delete this location?")) {
      deleteLocation(location.id);
    }
  };

  const handleEdit = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    onEdit();
  };

  const formattedCreatedAt = formatedDateTime(location.createdAt);
  const formattedUpdatedAt = formatedDateTime(location.updatedAt);
  const formattedDeletedAt = formatedDateTime(location.deletedAt);

  return (
    <Card
      key={location.id}
      className="group bg-linear-to-br
       from-slate-900/50
        to-slate-800/30
         border-slate-700/60
          hover:border-slate-600/80 hover:shadow-lg
           transition-all
            duration-300 hover:scale-[1.02] p-5"
    >
      {/* Header with name and status */}
      <div className="flex justify-between items-start mb-4">
        <div className="flex-1 min-w-0">
          <h3 className="text-lg font-semibold text-white mb-2 truncate">
            {location.name}
          </h3>

          {/* Address */}
          <div className="flex items-center text-slate-400 text-sm mb-2">
            <Globe className="w-4 h-4 mr-2 flex-shrink-0" />
            <span className="truncate">
              {location.address.city}, {location.address.street},{" "}
              {location.address.house}
            </span>
          </div>

          {/* Additional info grid */}
          <div className="grid gap-2 text-sm">
            {/* Timezone */}
            <div className="flex items-center text-slate-400">
              <Clock className="w-3.5 h-3.5 mr-2 flex-shrink-0" />
              <span className="truncate text-slate-300">
                {location.timeZone}
              </span>
            </div>
            
            {/* Дата создания */}
            <div className="flex items-center text-slate-400 col-span-2">
              <Calendar className="w-3.5 h-3.5 mr-2 flex-shrink-0" />
              <span className="text-slate-300">
                Created: {formattedCreatedAt}
              </span>
            </div>

            {/* Динамическая дата в зависимости от статуса */}
            {location.isActive
              ? location.updatedAt && (
                  <div className="flex items-center text-slate-400 col-span-2">
                    <Calendar className="w-3.5 h-3.5 mr-2 flex-shrink-0" />
                    <span className="text-slate-300">
                      Updated: {formattedUpdatedAt}
                    </span>
                  </div>
                )
              : location.deletedAt && (
                  <div className="flex items-center text-amber-400/80 col-span-2">
                    <Calendar className="w-3.5 h-3.5 mr-2 flex-shrink-0" />
                    <span className="text-amber-300/80">
                      Deleted: {formattedDeletedAt}
                    </span>
                  </div>
                )}
          </div>
        </div>

        {/* Status badge */}
        <div className="ml-3 flex-shrink-0">
          <span
            className={
              "inline-flex items-center px-3 py-1.5 rounded-full text-xs font-semibold transition-colors " +
              (location.isActive
                ? "bg-emerald-900/30 text-emerald-300 border border-emerald-700/40 hover:border-emerald-600/60"
                : "bg-amber-900/30 text-amber-300 border border-amber-700/40 hover:border-amber-600/60")
            }
          >
            <span
              className={
                "h-2 w-2 rounded-full mr-2 " +
                (location.isActive ? "bg-emerald-400" : "bg-amber-400")
              }
            />
            {location.isActive ? "Active" : "Inactive"}
          </span>
        </div>
      </div>

      {/* Action buttons */}
      <div className="flex justify-end items-center gap-2 pt-4 border-t border-slate-800/50">
        <Button
          variant="ghost"
          size="sm"
          className="h-9 px-3 text-slate-400 hover:text-blue-400 hover:bg-blue-900/20 border border-slate-700/50 hover:border-blue-700/50 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
          onClick={handleEdit}
          disabled={isPending}
        >
          <Edit2Icon className="h-4 w-4 mr-2" />
          Edit
        </Button>
        <Button
          variant="ghost"
          size="sm"
          className="h-9 px-3 text-slate-400 hover:text-red-400 hover:bg-red-900/20 border border-slate-700/50 hover:border-red-700/50 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
          onClick={handleDelete}
          disabled={isPending}
        >
          <Trash className="h-4 w-4 mr-2" />
          Delete
        </Button>
      </div>
    </Card>
  );
}
