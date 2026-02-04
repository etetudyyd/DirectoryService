import { Position } from "@/entities/positions/types";
import { useDeletePosition } from "../../features/positions/model/use-delete-position";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
} from "@/shared/components/ui/card";
import { Calendar, Edit2Icon, LetterText, Trash, Users } from "lucide-react";
import { Separator } from "@/shared/components/ui/separator";
import { Button } from "@/shared/components/ui/button";
import routes from "@/shared/routes";
import Link from "next/link";
import { DeleteConfirmationDialog } from "@/features/delete-confirmation-dialog";
import { useState } from "react";

type Props = {
  position: Position;
  onEdit: () => void;
};

const formatDateTime = (date: Date | string | null) => {
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

export default function PositionCard({ position, onEdit }: Props) {
  const { deletePosition, isPending } = useDeletePosition();

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleDelete = async () => {
    setLoading(true);
    try {
      await deletePosition(position.id);
    } finally {
      setLoading(false);
      setDeleteOpen(false);
    }
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDeleteOpen(true);
  };

  const handleEdit = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    onEdit();
  };

  const formattedCreatedAt = formatDateTime(position.createdAt);
  const formattedUpdatedAt = formatDateTime(position.updatedAt);
  const formattedDeletedAt = formatDateTime(position.deletedAt);
  const departmentCount = position.departmentsIds?.length || 0;

  return (
    <Link href={`${routes.positions}/${position.id}`}>
      <Card className="group relative overflow-hidden border-slate-700/50 bg-linear-to-br from-slate-900/50 to-slate-800/30 hover:shadow-xl transition-all duration-300 hover:border-slate-600/70 hover:scale-[1.02]">
        {/* Active location glow effect */}
        {position.isActive && (
          <div className="absolute inset-0 pointer-events-none opacity-0 group-hover:opacity-10 transition-opacity duration-300 bg-linear-to-br from-emerald-500/30 to-transparent" />
        )}

        {/* Вместо бейджа в CardHeader */}
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between">
            <div className="flex-1 min-w-0 pr-3">
              {" "}
              {/* Добавили padding-right */}
              <h3 className="text-lg font-semibold text-white truncate">
                {position.name}
              </h3>
            </div>

            <div className="shrink-0">
              <div className="flex flex-col items-end">
                <div
                  className={`h-2.5 w-2.5 rounded-full mb-1 ${
                    position.isActive ? "bg-emerald-500" : "bg-amber-500"
                  }`}
                />
              </div>
            </div>
          </div>
        </CardHeader>

        <CardContent className="pb-3">
          <div className="space-y-3">
            {/* Address */}
            <div className="flex items-start gap-2">
              <LetterText className="h-4 w-4 text-slate-400 mt-0.5 shrink-0" />
              <p className="text-sm text-slate-300">{position.description}</p>
            </div>

            <Separator className="bg-slate-800/50" />

            {/* Info grid */}
            <div className="grid grid-cols-2 gap-3">
              <div className="flex items-center gap-2">
                <Users className="h-4 w-4 text-slate-400 shrink-0" />
                <div className="min-w-0">
                  <p className="text-xs text-slate-500">Departments</p>
                  <p className="text-sm text-slate-300">
                    {departmentCount} {departmentCount === 1 ? "dept" : "depts"}
                  </p>
                </div>
              </div>

              <div className="flex items-center gap-2 col-span-2">
                <Calendar className="h-4 w-4 text-slate-400 shrink-0" />
                <div className="min-w-0">
                  <p className="text-xs text-slate-500">Created</p>
                  <p className="text-sm text-slate-300">{formattedCreatedAt}</p>
                </div>
              </div>

              {/* Updated or Deleted */}
              {position.isActive
                ? position.updatedAt && (
                    <div className="flex items-center gap-2 col-span-2">
                      <Calendar className="h-4 w-4 text-blue-400 shrink-0" />
                      <div className="min-w-0">
                        <p className="text-xs text-blue-500">Updated</p>
                        <p className="text-sm text-blue-300">
                          {formattedUpdatedAt}
                        </p>
                      </div>
                    </div>
                  )
                : position.deletedAt && (
                    <div className="flex items-center gap-2 col-span-2">
                      <Calendar className="h-4 w-4 text-amber-400 shrink-0" />
                      <div className="min-w-0">
                        <p className="text-xs text-amber-500">Deleted</p>
                        <p className="text-sm text-amber-300">
                          {formattedDeletedAt}
                        </p>
                      </div>
                    </div>
                  )}
            </div>
          </div>
        </CardContent>

        <CardFooter className="pt-3 border-t border-slate-800/50 bg-slate-900/20">
          <div className="flex w-full justify-end gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handleEdit}
              disabled={isPending}
              className="gap-2 border-slate-700/50 text-slate-400 hover:text-blue-400 hover:border-blue-700/50 hover:bg-blue-900/20 transition-all duration-200"
            >
              <Edit2Icon className="h-4 w-4" />
              Edit
            </Button>

            <Button
              variant="outline"
              size="sm"
              onClick={handleDeleteClick}
              disabled={isPending}
              className="gap-2 border-slate-700/50 text-slate-400 hover:text-red-400 hover:border-red-700/50 hover:bg-red-900/20 transition-all duration-200"
            >
              <Trash className="h-4 w-4" />
              Delete
            </Button>

            <DeleteConfirmationDialog
              open={deleteOpen}
              onOpenChange={setDeleteOpen}
              onConfirm={handleDelete}
              loading={loading}
              title={`Delete "${position.name}"?`}
              description="Are you sure you want to delete this position? This action cannot be undone."
            />
          </div>
        </CardFooter>
      </Card>
    </Link>
  );
}
