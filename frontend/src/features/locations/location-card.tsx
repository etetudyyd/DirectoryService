import { Card } from "@/shared/components/ui/card";
import { Location } from "@/entities/locations/types";
import { Button } from "@/shared/components/ui/button";
import { Trash } from "lucide-react";
import { useDeleteLocation } from "./model/use-delete-location";

type Props = {
  location: Location;
};

export default function LocationCard({ location }: Props) {
  const { deleteLocation, isPending } = useDeleteLocation();

  const handleDelete = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();

    if (confirm("Are you sure you want to delete this location?")) {
      deleteLocation(location.id);
    }
  };

  return (
    <Card key={location.id} className="bg-slate-900/40 border-slate-700 px-4">
      <div className="flex justify-between items-start">
        <div>
          <h3 className="text-lg font-medium text-white">{location.name}</h3>
          <div className="text-sm text-slate-400 mt-1">
            {location.address.city}, {location.address.street},{" "}
            {location.address.house}
          </div>
        </div>
        <div>
          {
            <span
              className={
                "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium " +
                (location.isActive
                  ? "bg-emerald-100 text-emerald-800"
                  : "bg-amber-100 text-amber-800")
              }
            >
              {location.isActive ? "Active" : "Deactived"}
            </span>
          }
        </div>
      </div>

      <div className="flex justify-end items-center mt-4">
        <Button
          variant="ghost"
          size="sm"
          className="h-8 w-8 text-destructive hover:text-white! hover:bg-red-500! transition-colors"
          onClick={handleDelete}
          disabled={isPending}
        >
          <Trash className="h-4 w-4" />
        </Button>
      </div>
    </Card>
  );
}
